using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using SDK.ERP.Application.Common;
using SDK.ERP.Infrastructure.Data;
using SDK.ERP.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure EF Core with Microsoft SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("SDK.ERP.Infrastructure");
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

// 2. Configure HTTP Context & Enterprise Multi-Tenant Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTenantProvider, CurrentTenantProvider>();
builder.Services.AddScoped<ICompanyContext, CompanyContext>();

// 3. Configure MVC & Razor Views
builder.Services.AddControllersWithViews();

// 4. Configure Session & Cookie Authentication
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication("ERP_Auth_Cookie")
    .AddCookie("ERP_Auth_Cookie", options =>
    {
        options.Cookie.Name = "SDK_ERP_Session";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// 5. Configure HTTP Request Pipeline & Security Headers
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 6. Database Initialization & Seeding on Startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dbCreator = Microsoft.EntityFrameworkCore.Infrastructure.AccessorExtensions.GetService<Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator>(db.Database) as Microsoft.EntityFrameworkCore.Storage.RelationalDatabaseCreator;
        if (dbCreator != null)
        {
            if (dbCreator.Exists() == false) dbCreator.Create();
            if (dbCreator.HasTables() == false) dbCreator.CreateTables();
        }
        else
        {
            db.Database.EnsureCreated();
        }

        // Independent isolated schema patches to guarantee critical columns exist
        async Task RunPatchAsync(string sql)
        {
            try { await db.Database.ExecuteSqlRawAsync(sql); }
            catch (Exception ex) { app.Logger.LogWarning("Schema patch note: {Msg}", ex.Message); }
        }

        // 1. Critical Vendor & Client columns (Fixes Invalid column name 'StateCode'/'StateName')
        await RunPatchAsync("IF OBJECT_ID('[master].[vendors]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'StateCode') ALTER TABLE [master].[vendors] ADD [StateCode] NVARCHAR(10) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[master].[vendors]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'StateName') ALTER TABLE [master].[vendors] ADD [StateName] NVARCHAR(100) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[master].[vendors]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'VendorCategory') ALTER TABLE [master].[vendors] ADD [VendorCategory] NVARCHAR(50) NULL DEFAULT 'GENERAL';");
        await RunPatchAsync("IF OBJECT_ID('[master].[vendors]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'MsmeType') ALTER TABLE [master].[vendors] ADD [MsmeType] NVARCHAR(50) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[master].[vendors]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'BankName') ALTER TABLE [master].[vendors] ADD [BankName] NVARCHAR(150) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[master].[vendors]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'BankAccountNo') ALTER TABLE [master].[vendors] ADD [BankAccountNo] NVARCHAR(50) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[master].[vendors]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'IfscCode') ALTER TABLE [master].[vendors] ADD [IfscCode] NVARCHAR(20) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[master].[vendors]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'PaymentTermsDays') ALTER TABLE [master].[vendors] ADD [PaymentTermsDays] INT NOT NULL DEFAULT 30;");

        await RunPatchAsync("IF OBJECT_ID('[master].[clients]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[clients]') AND name = 'StateName') ALTER TABLE [master].[clients] ADD [StateName] NVARCHAR(100) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[master].[clients]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[clients]') AND name = 'ClientCategory') ALTER TABLE [master].[clients] ADD [ClientCategory] NVARCHAR(30) NULL DEFAULT 'B2B';");
        await RunPatchAsync("IF OBJECT_ID('[master].[clients]') IS NOT NULL UPDATE [master].[clients] SET [ClientCategory] = 'B2B' WHERE [ClientCategory] IS NULL;");

        // 2. Ensure master.states table
        await RunPatchAsync(@"IF OBJECT_ID('[master].[states]') IS NULL
        BEGIN
            CREATE TABLE [master].[states] (
                [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [StateCode] NVARCHAR(10) NOT NULL,
                [StateName] NVARCHAR(100) NOT NULL,
                [StateType] NVARCHAR(50) NULL,
                [IsActive] BIT NOT NULL DEFAULT 1
            );
        END");

        // 3. Ensure DirectExpenses, CashWithdrawals, CashUtilisations, CashAllocations
        await RunPatchAsync(@"IF OBJECT_ID('[DirectExpenses]') IS NULL AND OBJECT_ID('[dbo].[DirectExpenses]') IS NULL
        BEGIN
            CREATE TABLE [dbo].[DirectExpenses] (
                [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [CompanyId] BIGINT NOT NULL,
                [ExpenseHead] NVARCHAR(250) NOT NULL,
                [ExpenseHeadId] BIGINT NULL,
                [IsVendorApplicable] BIT NOT NULL DEFAULT 0,
                [VendorId] BIGINT NULL,
                [LedgerId] BIGINT NULL,
                [LedgerName] NVARCHAR(250) NOT NULL DEFAULT '',
                [PaymentDate] DATETIME2 NOT NULL,
                [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                [IsPipelineProject] BIT NOT NULL DEFAULT 0,
                [ProjectId] BIGINT NULL,
                [Purpose] NVARCHAR(MAX) NOT NULL DEFAULT '',
                [Remarks] NVARCHAR(MAX) NULL,
                [Status] NVARCHAR(50) NOT NULL DEFAULT 'PENDING',
                [CreatedByUserId] BIGINT NULL,
                [CreatedByUserName] NVARCHAR(150) NULL,
                [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                [ApprovedByUserId] BIGINT NULL,
                [ApprovedByUserName] NVARCHAR(150) NULL,
                [ApprovedAt] DATETIME2 NULL,
                [ApprovalRemarks] NVARCHAR(MAX) NULL,
                [DeclinedByUserId] BIGINT NULL,
                [DeclinedByUserName] NVARCHAR(150) NULL,
                [DeclinedAt] DATETIME2 NULL,
                [DeclineReason] NVARCHAR(MAX) NULL,
                [SettledByUserId] BIGINT NULL,
                [SettledByUserName] NVARCHAR(150) NULL,
                [SettledAt] DATETIME2 NULL,
                [SettlementPaymentMode] NVARCHAR(100) NULL,
                [SettlementReference] NVARCHAR(150) NULL,
                [SettlementBankAccountId] BIGINT NULL,
                [SettlementBankName] NVARCHAR(150) NULL,
                [SettlementNotes] NVARCHAR(MAX) NULL,
                [JournalEntryId] BIGINT NULL
            );
        END");

        await RunPatchAsync(@"IF OBJECT_ID('[CashWithdrawals]') IS NULL AND OBJECT_ID('[dbo].[CashWithdrawals]') IS NULL
        BEGIN
            CREATE TABLE [dbo].[CashWithdrawals] (
                [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [CompanyId] BIGINT NOT NULL,
                [ReferenceNumber] NVARCHAR(50) NOT NULL,
                [WithdrawalDate] DATETIME2 NOT NULL,
                [SourceType] NVARCHAR(50) NOT NULL DEFAULT 'BANK_WITHDRAWAL',
                [BankAccountId] BIGINT NULL,
                [ChequeOrRefNo] NVARCHAR(100) NULL,
                [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                [UtilisedAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                [WithdrawnBy] NVARCHAR(150) NOT NULL DEFAULT '',
                [Purpose] NVARCHAR(MAX) NOT NULL DEFAULT '',
                [ApprovedBy] NVARCHAR(150) NOT NULL DEFAULT '',
                [BankStatementLineId] BIGINT NULL,
                [BankStatementRef] NVARCHAR(200) NULL,
                [Status] NVARCHAR(50) NOT NULL DEFAULT 'OPEN',
                [Notes] NVARCHAR(MAX) NULL,
                [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
            );
        END");

        await RunPatchAsync(@"IF OBJECT_ID('[CashUtilisations]') IS NULL AND OBJECT_ID('[dbo].[CashUtilisations]') IS NULL
        BEGIN
            CREATE TABLE [dbo].[CashUtilisations] (
                [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [CompanyId] BIGINT NOT NULL,
                [CashWithdrawalId] BIGINT NOT NULL,
                [UtilisationDate] DATETIME2 NOT NULL,
                [ExpenseHead] NVARCHAR(200) NOT NULL,
                [ExpenseHeadId] BIGINT NULL,
                [LedgerId] BIGINT NULL,
                [LedgerName] NVARCHAR(200) NOT NULL DEFAULT '',
                [ProjectId] BIGINT NULL,
                [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                [VoucherNumber] NVARCHAR(50) NOT NULL DEFAULT '',
                [SpentBy] NVARCHAR(150) NOT NULL DEFAULT '',
                [Description] NVARCHAR(MAX) NOT NULL DEFAULT '',
                [ReceiptAttachmentPath] NVARCHAR(500) NULL,
                [Status] NVARCHAR(50) NOT NULL DEFAULT 'RECORDED',
                [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
            );
        END");

        await RunPatchAsync(@"IF OBJECT_ID('[CashAllocations]') IS NULL AND OBJECT_ID('[dbo].[CashAllocations]') IS NULL
        BEGIN
            CREATE TABLE [dbo].[CashAllocations] (
                [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [CompanyId] BIGINT NOT NULL,
                [CashWithdrawalId] BIGINT NOT NULL,
                [RecipientUserId] BIGINT NULL,
                [RecipientName] NVARCHAR(150) NOT NULL,
                [AllocatedAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                [UtilisedAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                [AllocationDate] DATETIME2 NOT NULL,
                [Purpose] NVARCHAR(MAX) NOT NULL DEFAULT '',
                [Status] NVARCHAR(50) NOT NULL DEFAULT 'ALLOCATED',
                [Notes] NVARCHAR(MAX) NULL,
                [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
            );
        END");

        // 4. Ensure PurchaseOrderLogs audit table
        await RunPatchAsync(@"IF OBJECT_ID(N'[PurchaseOrderLogs]', N'U') IS NULL
        BEGIN
            CREATE TABLE [PurchaseOrderLogs] (
                [Id] BIGINT IDENTITY(1,1) NOT NULL,
                [PurchaseOrderId] BIGINT NULL,
                [PoId] BIGINT NULL,
                [ActionType] NVARCHAR(100) NOT NULL,
                [Title] NVARCHAR(200) NOT NULL,
                [Description] NVARCHAR(MAX) NULL,
                [Amount] DECIMAL(18,2) NULL,
                [PaymentMode] NVARCHAR(50) NULL,
                [VoucherOrRefNo] NVARCHAR(100) NULL,
                [PerformedById] BIGINT NULL,
                [PerformedByName] NVARCHAR(150) NOT NULL,
                [PerformedByRole] NVARCHAR(50) NULL,
                [Remarks] NVARCHAR(MAX) NULL,
                [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                CONSTRAINT [PK_PurchaseOrderLogs] PRIMARY KEY ([Id])
            );
            CREATE INDEX [IX_PurchaseOrderLogs_PoId_CreatedAt] ON [PurchaseOrderLogs] ([PoId], [CreatedAt]);
        END");
        await RunPatchAsync("IF OBJECT_ID(N'[PurchaseOrderLogs]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderLogs]') AND name = 'PoId') ALTER TABLE [PurchaseOrderLogs] ADD [PoId] BIGINT NULL;");
        await RunPatchAsync("IF OBJECT_ID(N'[PurchaseOrderLogs]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderLogs]') AND name = 'PurchaseOrderId') ALTER TABLE [PurchaseOrderLogs] ADD [PurchaseOrderId] BIGINT NULL;");

        // 5. Ensure PurchaseOrders columns exist
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'PoType') ALTER TABLE [PurchaseOrders] ADD [PoType] NVARCHAR(100) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'ShipTo') ALTER TABLE [PurchaseOrders] ADD [ShipTo] NVARCHAR(MAX) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'Remarks') ALTER TABLE [PurchaseOrders] ADD [Remarks] NVARCHAR(MAX) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'TermsConditions') ALTER TABLE [PurchaseOrders] ADD [TermsConditions] NVARCHAR(MAX) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'CgstAmount') ALTER TABLE [PurchaseOrders] ADD [CgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'SgstAmount') ALTER TABLE [PurchaseOrders] ADD [SgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'IgstAmount') ALTER TABLE [PurchaseOrders] ADD [IgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'ApprovedAt') ALTER TABLE [PurchaseOrders] ADD [ApprovedAt] DATETIME2 NULL;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'RejectionReason') ALTER TABLE [PurchaseOrders] ADD [RejectionReason] NVARCHAR(MAX) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'MarkedForPaymentAt') ALTER TABLE [PurchaseOrders] ADD [MarkedForPaymentAt] DATETIME2 NULL;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'MarkedPaymentType') ALTER TABLE [PurchaseOrders] ADD [MarkedPaymentType] NVARCHAR(50) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'MarkedAmount') ALTER TABLE [PurchaseOrders] ADD [MarkedAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'PaymentApprovedBy') ALTER TABLE [PurchaseOrders] ADD [PaymentApprovedBy] BIGINT NULL;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'PaymentApprovedByName') ALTER TABLE [PurchaseOrders] ADD [PaymentApprovedByName] NVARCHAR(150) NULL;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'PaymentApprovedAt') ALTER TABLE [PurchaseOrders] ADD [PaymentApprovedAt] DATETIME2 NULL;");
        await RunPatchAsync("IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[PurchaseOrders]') AND name = 'PaymentApprovalRemarks') ALTER TABLE [PurchaseOrders] ADD [PaymentApprovalRemarks] NVARCHAR(MAX) NULL;");

        // Ensure PAN, GSTIN, Company & User columns accommodate flexible inputs
        try
        {
            await db.Database.ExecuteSqlRawAsync(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[clients]') AND name = 'Pan')
                    ALTER TABLE [master].[clients] ALTER COLUMN [Pan] NVARCHAR(50) NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[clients]') AND name = 'Gstin')
                    ALTER TABLE [master].[clients] ALTER COLUMN [Gstin] NVARCHAR(50) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[clients]') AND name = 'StateName')
                    ALTER TABLE [master].[clients] ADD [StateName] NVARCHAR(100) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[clients]') AND name = 'ClientCategory')
                    ALTER TABLE [master].[clients] ADD [ClientCategory] NVARCHAR(30) NULL DEFAULT 'B2B';
                UPDATE [master].[clients] SET [ClientCategory] = 'B2B' WHERE [ClientCategory] IS NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'Pan')
                    ALTER TABLE [master].[vendors] ALTER COLUMN [Pan] NVARCHAR(50) NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'Gstin')
                    ALTER TABLE [master].[vendors] ALTER COLUMN [Gstin] NVARCHAR(50) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'StateCode')
                    ALTER TABLE [master].[vendors] ADD [StateCode] NVARCHAR(10) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'StateName')
                    ALTER TABLE [master].[vendors] ADD [StateName] NVARCHAR(100) NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[projects].[projects]') AND name = 'ManagerId')
                    ALTER TABLE [projects].[projects] ALTER COLUMN [ManagerId] BIGINT NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[GstTransactions]') AND name = 'VoucherId')
                    ALTER TABLE [GstTransactions] ALTER COLUMN [VoucherId] BIGINT NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[tax].[gst_transactions]') AND name = 'VoucherId')
                    ALTER TABLE [tax].[gst_transactions] ALTER COLUMN [VoucherId] BIGINT NULL;
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_journal_entries_FinancialYears_FinancialYearId')
                    ALTER TABLE [accounting].[journal_entries] DROP CONSTRAINT [FK_journal_entries_FinancialYears_FinancialYearId];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_journal_entries_FinancialYears_FinancialYearId')
                    ALTER TABLE [journal_entries] DROP CONSTRAINT [FK_journal_entries_FinancialYears_FinancialYearId];
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[accounting].[journal_entries]') AND name = 'FinancialYearId')
                    ALTER TABLE [accounting].[journal_entries] ALTER COLUMN [FinancialYearId] INT NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[journal_entries]') AND name = 'FinancialYearId')
                    ALTER TABLE [journal_entries] ALTER COLUMN [FinancialYearId] INT NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[accounting].[journal_entries]') AND name = 'CreatorId')
                    ALTER TABLE [accounting].[journal_entries] ALTER COLUMN [CreatorId] BIGINT NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[journal_entries]') AND name = 'CreatorId')
                    ALTER TABLE [journal_entries] ALTER COLUMN [CreatorId] BIGINT NULL;

                -- Ensure Self-Service Company Profile Columns
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'BrandShortName')
                    ALTER TABLE [admin].[companies] ADD [BrandShortName] NVARCHAR(50) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'Tagline')
                    ALTER TABLE [admin].[companies] ADD [Tagline] NVARCHAR(150) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'Industry')
                    ALTER TABLE [admin].[companies] ADD [Industry] NVARCHAR(100) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'Email')
                    ALTER TABLE [admin].[companies] ADD [Email] NVARCHAR(150) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'Phone')
                    ALTER TABLE [admin].[companies] ADD [Phone] NVARCHAR(50) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'Website')
                    ALTER TABLE [admin].[companies] ADD [Website] NVARCHAR(200) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'AddressLine1')
                    ALTER TABLE [admin].[companies] ADD [AddressLine1] NVARCHAR(255) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'AddressLine2')
                    ALTER TABLE [admin].[companies] ADD [AddressLine2] NVARCHAR(255) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'City')
                    ALTER TABLE [admin].[companies] ADD [City] NVARCHAR(100) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'State')
                    ALTER TABLE [admin].[companies] ADD [State] NVARCHAR(100) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'StateCode')
                    ALTER TABLE [admin].[companies] ADD [StateCode] NVARCHAR(10) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'Pincode')
                    ALTER TABLE [admin].[companies] ADD [Pincode] NVARCHAR(20) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'Cin')
                    ALTER TABLE [admin].[companies] ADD [Cin] NVARCHAR(50) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'MsmeUdyamNo')
                    ALTER TABLE [admin].[companies] ADD [MsmeUdyamNo] NVARCHAR(50) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'BankName')
                    ALTER TABLE [admin].[companies] ADD [BankName] NVARCHAR(150) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'BankAccountNumber')
                    ALTER TABLE [admin].[companies] ADD [BankAccountNumber] NVARCHAR(50) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'BankIfsc')
                    ALTER TABLE [admin].[companies] ADD [BankIfsc] NVARCHAR(30) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'BankBranch')
                    ALTER TABLE [admin].[companies] ADD [BankBranch] NVARCHAR(150) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'UpiId')
                    ALTER TABLE [admin].[companies] ADD [UpiId] NVARCHAR(100) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'LogoUrl')
                    ALTER TABLE [admin].[companies] ADD [LogoUrl] NVARCHAR(500) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'AuthorizedSignatoryName')
                    ALTER TABLE [admin].[companies] ADD [AuthorizedSignatoryName] NVARCHAR(100) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'AuthorizedSignatoryDesignation')
                    ALTER TABLE [admin].[companies] ADD [AuthorizedSignatoryDesignation] NVARCHAR(100) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[companies]') AND name = 'TermsAndConditions')
                    ALTER TABLE [admin].[companies] ADD [TermsAndConditions] NVARCHAR(MAX) NULL;

                -- Ensure Self-Service User Profile Columns
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[users]') AND name = 'FullName')
                    ALTER TABLE [admin].[users] ADD [FullName] NVARCHAR(150) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[users]') AND name = 'Designation')
                    ALTER TABLE [admin].[users] ADD [Designation] NVARCHAR(100) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[users]') AND name = 'PhoneNumber')
                    ALTER TABLE [admin].[users] ADD [PhoneNumber] NVARCHAR(50) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[admin].[users]') AND name = 'AvatarUrl')
                    ALTER TABLE [admin].[users] ADD [AvatarUrl] NVARCHAR(500) NULL;

                -- Ensure Project Site Expense Columns
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[ProjectExpenses]') AND name = 'Description')
                    ALTER TABLE [ProjectExpenses] ADD [Description] NVARCHAR(500) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[ProjectExpenses]') AND name = 'Description')
                    ALTER TABLE [project].[ProjectExpenses] ADD [Description] NVARCHAR(500) NULL;

                -- Ensure ProjectTypes Master Table
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProjectTypes')
                BEGIN
                    CREATE TABLE [dbo].[ProjectTypes] (
                        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [Code] NVARCHAR(50) NOT NULL,
                        [Name] NVARCHAR(150) NOT NULL,
                        [Description] NVARCHAR(500) NULL,
                        [IsActive] BIT NOT NULL DEFAULT 1
                    );
                END

                -- Ensure ExpenseTypes Master Table
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ExpenseTypes')
                BEGIN
                    CREATE TABLE [dbo].[ExpenseTypes] (
                        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [Name] NVARCHAR(250) NOT NULL,
                        [Description] NVARCHAR(1000) NULL,
                        [IsProjectRelated] BIT NOT NULL DEFAULT 0,
                        [ReceiveDirectPayments] BIT NOT NULL DEFAULT 0,
                        [OpeningAmount] DECIMAL(18,2) NULL,
                        [IsActive] BIT NOT NULL DEFAULT 1,
                        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                    );
                END

                -- Ensure Project Delivery Challan Columns
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[ProjectDeliveries]') AND name = 'MaterialSummary')
                    ALTER TABLE [ProjectDeliveries] ADD [MaterialSummary] NVARCHAR(1000) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[ProjectDeliveries]') AND name = 'MaterialSummary')
                    ALTER TABLE [project].[ProjectDeliveries] ADD [MaterialSummary] NVARCHAR(1000) NULL;

                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[ProjectDeliveries]') AND name = 'AttachmentDocId')
                    ALTER TABLE [ProjectDeliveries] ADD [AttachmentDocId] BIGINT NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[ProjectDeliveries]') AND name = 'AttachmentDocId')
                    ALTER TABLE [project].[ProjectDeliveries] ADD [AttachmentDocId] BIGINT NULL;

                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[DocumentAttachments]') AND name = 'UploaderId' AND is_nullable = 0)
                    ALTER TABLE [DocumentAttachments] ALTER COLUMN [UploaderId] BIGINT NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[DocumentAttachments]') AND name = 'UploadedBy' AND is_nullable = 0)
                    ALTER TABLE [DocumentAttachments] ALTER COLUMN [UploadedBy] BIGINT NULL;

                -- Ensure Project Division, Description & PhysicalFileStatus Columns
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[projects]') AND name = 'Division')
                    ALTER TABLE [project].[projects] ADD [Division] NVARCHAR(200) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[projects].[projects]') AND name = 'Division')
                    ALTER TABLE [projects].[projects] ADD [Division] NVARCHAR(200) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Projects]') AND name = 'Division')
                    ALTER TABLE [Projects] ADD [Division] NVARCHAR(200) NULL;

                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[projects]') AND name = 'Description')
                    ALTER TABLE [project].[projects] ADD [Description] NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[projects].[projects]') AND name = 'Description')
                    ALTER TABLE [projects].[projects] ADD [Description] NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Projects]') AND name = 'Description')
                    ALTER TABLE [Projects] ADD [Description] NVARCHAR(MAX) NULL;

                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[projects]') AND name = 'PhysicalFileStatus')
                    ALTER TABLE [project].[projects] ADD [PhysicalFileStatus] NVARCHAR(100) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[projects].[projects]') AND name = 'PhysicalFileStatus')
                    ALTER TABLE [projects].[projects] ADD [PhysicalFileStatus] NVARCHAR(100) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Projects]') AND name = 'PhysicalFileStatus')
                    ALTER TABLE [Projects] ADD [PhysicalFileStatus] NVARCHAR(100) NULL;

                -- Ensure SalesInvoice Reference, Work Order, Terms & Deductions Columns
                IF OBJECT_ID('[sales].[sales_invoices]') IS NOT NULL
                BEGIN
                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'ProjectId' AND is_nullable = 0)
                        ALTER TABLE [sales].[sales_invoices] ALTER COLUMN [ProjectId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'WorkOrderNo')
                        ALTER TABLE [sales].[sales_invoices] ADD [WorkOrderNo] NVARCHAR(100) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'WorkOrderDate')
                        ALTER TABLE [sales].[sales_invoices] ADD [WorkOrderDate] DATETIME2 NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'BillingAttention')
                        ALTER TABLE [sales].[sales_invoices] ADD [BillingAttention] NVARCHAR(250) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'Remarks')
                        ALTER TABLE [sales].[sales_invoices] ADD [Remarks] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'DeductionRemarks')
                        ALTER TABLE [sales].[sales_invoices] ADD [DeductionRemarks] NVARCHAR(250) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'DeductionAmount')
                        ALTER TABLE [sales].[sales_invoices] ADD [DeductionAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                END
                IF OBJECT_ID('[SalesInvoices]') IS NOT NULL
                BEGIN
                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[SalesInvoices]') AND name = 'ProjectId' AND is_nullable = 0)
                        ALTER TABLE [SalesInvoices] ALTER COLUMN [ProjectId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[SalesInvoices]') AND name = 'WorkOrderNo')
                        ALTER TABLE [SalesInvoices] ADD [WorkOrderNo] NVARCHAR(100) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[SalesInvoices]') AND name = 'WorkOrderDate')
                        ALTER TABLE [SalesInvoices] ADD [WorkOrderDate] DATETIME2 NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[SalesInvoices]') AND name = 'BillingAttention')
                        ALTER TABLE [SalesInvoices] ADD [BillingAttention] NVARCHAR(250) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[SalesInvoices]') AND name = 'Remarks')
                        ALTER TABLE [SalesInvoices] ADD [Remarks] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[SalesInvoices]') AND name = 'DeductionRemarks')
                        ALTER TABLE [SalesInvoices] ADD [DeductionRemarks] NVARCHAR(250) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[SalesInvoices]') AND name = 'DeductionAmount')
                        ALTER TABLE [SalesInvoices] ADD [DeductionAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                END

                -- Ensure SalesInvoiceItem Tax Columns
                IF OBJECT_ID('[sales].[sales_invoice_items]') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoice_items]') AND name = 'GstRate')
                        ALTER TABLE [sales].[sales_invoice_items] ADD [GstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoice_items]') AND name = 'IgstRate')
                        ALTER TABLE [sales].[sales_invoice_items] ADD [IgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoice_items]') AND name = 'CgstRate')
                        ALTER TABLE [sales].[sales_invoice_items] ADD [CgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoice_items]') AND name = 'SgstRate')
                        ALTER TABLE [sales].[sales_invoice_items] ADD [SgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoice_items]') AND name = 'TaxRateId' AND is_nullable = 0)
                        ALTER TABLE [sales].[sales_invoice_items] ALTER COLUMN [TaxRateId] INT NULL;
                END
                IF OBJECT_ID('[SalesInvoiceItems]') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[SalesInvoiceItems]') AND name = 'GstRate')
                        ALTER TABLE [SalesInvoiceItems] ADD [GstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[SalesInvoiceItems]') AND name = 'IgstRate')
                        ALTER TABLE [SalesInvoiceItems] ADD [IgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[SalesInvoiceItems]') AND name = 'CgstRate')
                        ALTER TABLE [SalesInvoiceItems] ADD [CgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[SalesInvoiceItems]') AND name = 'SgstRate')
                        ALTER TABLE [SalesInvoiceItems] ADD [SgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[SalesInvoiceItems]') AND name = 'TaxRateId' AND is_nullable = 0)
                        ALTER TABLE [SalesInvoiceItems] ALTER COLUMN [TaxRateId] INT NULL;
                END

                -- Ensure CustomerReceipts Direct Payment Columns
                IF OBJECT_ID('[CustomerReceipts]') IS NOT NULL
                BEGIN
                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'ClientId' AND is_nullable = 0)
                        ALTER TABLE [CustomerReceipts] ALTER COLUMN [ClientId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'ExpenseHead')
                        ALTER TABLE [CustomerReceipts] ADD [ExpenseHead] NVARCHAR(200) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'Remarks')
                        ALTER TABLE [CustomerReceipts] ADD [Remarks] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'ReceiptDocId')
                        ALTER TABLE [CustomerReceipts] ADD [ReceiptDocId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'ProjectId')
                        ALTER TABLE [CustomerReceipts] ADD [ProjectId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'InvoiceId')
                        ALTER TABLE [CustomerReceipts] ADD [InvoiceId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'IsAdvance')
                        ALTER TABLE [CustomerReceipts] ADD [IsAdvance] BIT NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'TdsAmount')
                        ALTER TABLE [CustomerReceipts] ADD [TdsAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'GstTdsAmount')
                        ALTER TABLE [CustomerReceipts] ADD [GstTdsAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'SecurityDepositAmount')
                        ALTER TABLE [CustomerReceipts] ADD [SecurityDepositAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'OtherDeductionAmount')
                        ALTER TABLE [CustomerReceipts] ADD [OtherDeductionAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'NetAmountReceived')
                        ALTER TABLE [CustomerReceipts] ADD [NetAmountReceived] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[CustomerReceipts]') AND name = 'TotalAmountReceived')
                        ALTER TABLE [CustomerReceipts] ADD [TotalAmountReceived] DECIMAL(18,2) NOT NULL DEFAULT 0;
                END
            ");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Schema adjustment SQL error: {Message}", ex.Message);
        }

        // Dedicated PurchaseOrders, VendorPayments & PurchaseOrderLogs Complete Schema Migration
        try
        {
            await db.Database.ExecuteSqlRawAsync(@"
                IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL OR OBJECT_ID('[dbo].[PurchaseOrders]') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'PoType')
                        ALTER TABLE [PurchaseOrders] ADD [PoType] NVARCHAR(100) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'ShipTo')
                        ALTER TABLE [PurchaseOrders] ADD [ShipTo] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'Remarks')
                        ALTER TABLE [PurchaseOrders] ADD [Remarks] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'TermsConditions')
                        ALTER TABLE [PurchaseOrders] ADD [TermsConditions] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'CgstAmount')
                        ALTER TABLE [PurchaseOrders] ADD [CgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'SgstAmount')
                        ALTER TABLE [PurchaseOrders] ADD [SgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'IgstAmount')
                        ALTER TABLE [PurchaseOrders] ADD [IgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'PaidAmount')
                        ALTER TABLE [PurchaseOrders] ADD [PaidAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'MarkedAmount')
                        ALTER TABLE [PurchaseOrders] ADD [MarkedAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'MarkedPaymentType')
                        ALTER TABLE [PurchaseOrders] ADD [MarkedPaymentType] NVARCHAR(50) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'ExpenseHead')
                        ALTER TABLE [PurchaseOrders] ADD [ExpenseHead] NVARCHAR(250) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'CreatedBy')
                        ALTER TABLE [PurchaseOrders] ADD [CreatedBy] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'CreatedByName')
                        ALTER TABLE [PurchaseOrders] ADD [CreatedByName] NVARCHAR(150) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'CreatedByRole')
                        ALTER TABLE [PurchaseOrders] ADD [CreatedByRole] NVARCHAR(50) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'ApprovedAt')
                        ALTER TABLE [PurchaseOrders] ADD [ApprovedAt] DATETIME2 NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'RejectionReason')
                        ALTER TABLE [PurchaseOrders] ADD [RejectionReason] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'MarkedForPaymentAt')
                        ALTER TABLE [PurchaseOrders] ADD [MarkedForPaymentAt] DATETIME2 NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'PaymentApprovedBy')
                        ALTER TABLE [PurchaseOrders] ADD [PaymentApprovedBy] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'PaymentApprovedByName')
                        ALTER TABLE [PurchaseOrders] ADD [PaymentApprovedByName] NVARCHAR(150) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'PaymentApprovedAt')
                        ALTER TABLE [PurchaseOrders] ADD [PaymentApprovedAt] DATETIME2 NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'PaymentApprovalRemarks')
                        ALTER TABLE [PurchaseOrders] ADD [PaymentApprovalRemarks] NVARCHAR(MAX) NULL;
                END

                IF OBJECT_ID('[VendorPayments]') IS NOT NULL OR OBJECT_ID('[dbo].[VendorPayments]') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[VendorPayments]') OR object_id = OBJECT_ID('[dbo].[VendorPayments]')) AND name = 'PoId')
                        ALTER TABLE [VendorPayments] ADD [PoId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[VendorPayments]') OR object_id = OBJECT_ID('[dbo].[VendorPayments]')) AND name = 'Remarks')
                        ALTER TABLE [VendorPayments] ADD [Remarks] NVARCHAR(MAX) NULL;
                END

                IF OBJECT_ID('[PurchaseOrderItems]') IS NOT NULL OR OBJECT_ID('[dbo].[PurchaseOrderItems]') IS NOT NULL
                BEGIN
                    IF EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'PoId' AND is_nullable = 0)
                        ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [PoId] BIGINT NULL;
                    IF EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'PurchaseOrderId' AND is_nullable = 0)
                        ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [PurchaseOrderId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'PurchaseOrderId')
                        ALTER TABLE [PurchaseOrderItems] ADD [PurchaseOrderId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'GstRate')
                        ALTER TABLE [PurchaseOrderItems] ADD [GstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'IgstRate')
                        ALTER TABLE [PurchaseOrderItems] ADD [IgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'CgstRate')
                        ALTER TABLE [PurchaseOrderItems] ADD [CgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'SgstRate')
                        ALTER TABLE [PurchaseOrderItems] ADD [SgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'TaxAmount')
                        ALTER TABLE [PurchaseOrderItems] ADD [TaxAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'TaxRateId' AND is_nullable = 0)
                        ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [TaxRateId] INT NULL;
                    IF EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'UnitId' AND is_nullable = 0)
                        ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [UnitId] INT NULL;
                END

                IF OBJECT_ID(N'[PurchaseOrderLogs]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [PurchaseOrderLogs] (
                        [Id] BIGINT NOT NULL IDENTITY,
                        [PoId] BIGINT NOT NULL,
                        [PurchaseOrderId] BIGINT NULL,
                        [ActionType] NVARCHAR(50) NOT NULL,
                        [Title] NVARCHAR(200) NOT NULL,
                        [Description] NVARCHAR(MAX) NULL,
                        [Amount] DECIMAL(18,2) NULL,
                        [PaymentMode] NVARCHAR(50) NULL,
                        [VoucherOrRefNo] NVARCHAR(100) NULL,
                        [PerformedById] BIGINT NULL,
                        [PerformedByName] NVARCHAR(150) NOT NULL,
                        [PerformedByRole] NVARCHAR(50) NULL,
                        [Remarks] NVARCHAR(MAX) NULL,
                        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                        CONSTRAINT [PK_PurchaseOrderLogs] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_PurchaseOrderLogs_PurchaseOrders_PoId] FOREIGN KEY ([PoId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE CASCADE
                    );
                    CREATE INDEX [IX_PurchaseOrderLogs_PoId_CreatedAt] ON [PurchaseOrderLogs] ([PoId], [CreatedAt]);
                END
                ELSE
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderLogs]') AND name = 'PurchaseOrderId')
                        ALTER TABLE [PurchaseOrderLogs] ADD [PurchaseOrderId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderLogs]') AND name = 'PoId')
                        ALTER TABLE [PurchaseOrderLogs] ADD [PoId] BIGINT NULL;
                END

                IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL OR OBJECT_ID('[dbo].[PurchaseOrders]') IS NOT NULL
                BEGIN
                    IF EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'PaidAmount')
                        EXEC('UPDATE [PurchaseOrders] SET [PaidAmount] = [TotalPoValue] WHERE [ApprovalStatus] = ''PAID'' AND ([PaidAmount] IS NULL OR [PaidAmount] = 0)');
                    IF EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'MarkedAmount')
                        EXEC('UPDATE [PurchaseOrders] SET [MarkedAmount] = [TotalPoValue], [MarkedPaymentType] = ''FULL'' WHERE [ApprovalStatus] = ''MARKED_FOR_PAYMENT'' AND ([MarkedAmount] IS NULL OR [MarkedAmount] = 0)');
                END
            ");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "PurchaseOrders schema migration SQL error: {Message}", ex.Message);
        }

        // Dedicated DirectExpenses Schema Migration & Auto-Creation
        try
        {
            await db.Database.ExecuteSqlRawAsync(@"
                IF OBJECT_ID('[DirectExpenses]') IS NULL AND OBJECT_ID('[dbo].[DirectExpenses]') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[DirectExpenses] (
                        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [CompanyId] BIGINT NOT NULL,
                        [ExpenseHead] NVARCHAR(250) NOT NULL,
                        [ExpenseHeadId] BIGINT NULL,
                        [IsVendorApplicable] BIT NOT NULL DEFAULT 0,
                        [VendorId] BIGINT NULL,
                        [LedgerId] BIGINT NULL,
                        [LedgerName] NVARCHAR(250) NOT NULL DEFAULT '',
                        [PaymentDate] DATETIME2 NOT NULL,
                        [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                        [IsPipelineProject] BIT NOT NULL DEFAULT 0,
                        [ProjectId] BIGINT NULL,
                        [Purpose] NVARCHAR(MAX) NOT NULL DEFAULT '',
                        [Remarks] NVARCHAR(MAX) NULL,
                        [Status] NVARCHAR(50) NOT NULL DEFAULT 'PENDING',
                        [CreatedByUserId] BIGINT NULL,
                        [CreatedByUserName] NVARCHAR(150) NULL,
                        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                        [ApprovedByUserId] BIGINT NULL,
                        [ApprovedByUserName] NVARCHAR(150) NULL,
                        [ApprovedAt] DATETIME2 NULL,
                        [ApprovalRemarks] NVARCHAR(MAX) NULL,
                        [DeclinedByUserId] BIGINT NULL,
                        [DeclinedByUserName] NVARCHAR(150) NULL,
                        [DeclinedAt] DATETIME2 NULL,
                        [DeclineReason] NVARCHAR(MAX) NULL,
                        [SettledByUserId] BIGINT NULL,
                        [SettledByUserName] NVARCHAR(150) NULL,
                        [SettledAt] DATETIME2 NULL,
                        [SettlementPaymentMode] NVARCHAR(100) NULL,
                        [SettlementReference] NVARCHAR(150) NULL,
                        [SettlementBankAccountId] BIGINT NULL,
                        [SettlementBankName] NVARCHAR(150) NULL,
                        [SettlementNotes] NVARCHAR(MAX) NULL,
                        [JournalEntryId] BIGINT NULL
                    );
                END
            ");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "DirectExpenses schema migration SQL error: {Message}", ex.Message);
        }

        // Dedicated Petty Cash Management (CashWithdrawals, CashUtilisations, CashAllocations) Schema Migration
        try
        {
            await db.Database.ExecuteSqlRawAsync(@"
                -- Ensure BankAccounts Table
                IF OBJECT_ID('[BankAccounts]') IS NULL AND OBJECT_ID('[dbo].[BankAccounts]') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[BankAccounts] (
                        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [CompanyId] BIGINT NOT NULL DEFAULT 1,
                        [GlAccountId] BIGINT NOT NULL DEFAULT 1,
                        [BankName] NVARCHAR(150) NOT NULL,
                        [BranchName] NVARCHAR(150) NULL,
                        [AccountNumber] NVARCHAR(50) NOT NULL,
                        [IfscCode] NVARCHAR(30) NOT NULL,
                        [AccountType] NVARCHAR(50) NOT NULL DEFAULT 'CURRENT',
                        [BookBalance] DECIMAL(18,2) NOT NULL DEFAULT 0,
                        [IsActive] BIT NOT NULL DEFAULT 1
                    );
                END

                -- CashWithdrawals
                IF OBJECT_ID('[CashWithdrawals]') IS NULL AND OBJECT_ID('[dbo].[CashWithdrawals]') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[CashWithdrawals] (
                        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [CompanyId] BIGINT NOT NULL DEFAULT 1,
                        [ReferenceNumber] NVARCHAR(100) NOT NULL,
                        [WithdrawalDate] DATETIME2 NOT NULL,
                        [SourceType] NVARCHAR(50) NOT NULL DEFAULT 'BANK_WITHDRAWAL',
                        [BankAccountId] BIGINT NULL,
                        [ChequeOrRefNo] NVARCHAR(100) NULL,
                        [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                        [UtilisedAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                        [WithdrawnBy] NVARCHAR(150) NOT NULL,
                        [Purpose] NVARCHAR(500) NOT NULL,
                        [ApprovedBy] NVARCHAR(150) NOT NULL,
                        [BankStatementLineId] BIGINT NULL,
                        [BankStatementRef] NVARCHAR(200) NULL,
                        [Status] NVARCHAR(50) NOT NULL DEFAULT 'OPEN',
                        [Notes] NVARCHAR(1000) NULL,
                        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                    );
                END

                -- CashUtilisations
                IF OBJECT_ID('[CashUtilisations]') IS NULL AND OBJECT_ID('[dbo].[CashUtilisations]') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[CashUtilisations] (
                        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [CompanyId] BIGINT NOT NULL DEFAULT 1,
                        [VoucherNumber] NVARCHAR(100) NOT NULL,
                        [ExpenseDate] DATETIME2 NOT NULL,
                        [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                        [AllocatedAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                        [Payee] NVARCHAR(200) NOT NULL,
                        [ExpenseCategory] NVARCHAR(150) NOT NULL,
                        [ExpenseAccountId] BIGINT NULL,
                        [ProjectId] BIGINT NULL,
                        [CostCenter] NVARCHAR(100) NULL,
                        [Purpose] NVARCHAR(500) NOT NULL,
                        [AttachmentFileName] NVARCHAR(255) NULL,
                        [AttachmentFilePath] NVARCHAR(500) NULL,
                        [PaidBy] NVARCHAR(150) NOT NULL,
                        [ApprovedBy] NVARCHAR(150) NOT NULL,
                        [Status] NVARCHAR(50) NOT NULL DEFAULT 'DRAFT',
                        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                    );
                END

                -- CashAllocations
                IF OBJECT_ID('[CashAllocations]') IS NULL AND OBJECT_ID('[dbo].[CashAllocations]') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[CashAllocations] (
                        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [CompanyId] BIGINT NOT NULL DEFAULT 1,
                        [WithdrawalId] BIGINT NOT NULL,
                        [UtilisationId] BIGINT NOT NULL,
                        [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                        [AllocationDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                        [AllocatedBy] NVARCHAR(150) NULL,
                        [IsManualOverride] BIT NOT NULL DEFAULT 0,
                        [Notes] NVARCHAR(500) NULL
                    );
                END
            ");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Petty Cash schema migration SQL error: {Message}", ex.Message);
        }

        await SeedData.InitializeAsync(db);
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning("Database initialization skipped or pending: {Message}", ex.Message);
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Security Response Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
