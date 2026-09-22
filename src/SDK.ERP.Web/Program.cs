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
        db.Database.EnsureCreated();

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
