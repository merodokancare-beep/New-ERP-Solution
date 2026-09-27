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

        // Ensure PAN, GSTIN, Company & User columns accommodate flexible inputs
        try
        {
            await db.Database.ExecuteSqlRawAsync(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[clients]') AND name = 'Pan')
                    ALTER TABLE [master].[clients] ALTER COLUMN [Pan] NVARCHAR(50) NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[clients]') AND name = 'Gstin')
                    ALTER TABLE [master].[clients] ALTER COLUMN [Gstin] NVARCHAR(50) NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'Pan')
                    ALTER TABLE [master].[vendors] ALTER COLUMN [Pan] NVARCHAR(50) NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'Gstin')
                    ALTER TABLE [master].[vendors] ALTER COLUMN [Gstin] NVARCHAR(50) NULL;
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
        catch { }

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
