import os

def create_directory(path):
    if not os.path.exists(path):
        os.makedirs(path)

create_directory("src/SDK.ERP.Infrastructure/Data")
create_directory("src/SDK.ERP.Infrastructure/Configurations")

# ApplicationDbContext.cs
with open("src/SDK.ERP.Infrastructure/Data/ApplicationDbContext.cs", "w", encoding="utf-8") as f:
    f.write('''using Microsoft.EntityFrameworkCore;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;
using SDK.ERP.Domain.Entities.Sales;
using SDK.ERP.Domain.Entities.Procurement;
using SDK.ERP.Domain.Entities.Inventory;
using SDK.ERP.Domain.Entities.Accounting;
using SDK.ERP.Domain.Entities.Tax;
using SDK.ERP.Domain.Entities.Banking;
using SDK.ERP.Domain.Entities.Payroll;
using SDK.ERP.Domain.Entities.AssetsAndSupport;

namespace SDK.ERP.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Domain 1: Admin & Security
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ApprovalRule> ApprovalRules => Set<ApprovalRule>();
    public DbSet<ApprovalRequest> ApprovalRequests => Set<ApprovalRequest>();
    public DbSet<DocumentAttachment> DocumentAttachments => Set<DocumentAttachment>();

    // Domain 2: Master Data
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();
    public DbSet<ItemUnit> ItemUnits => Set<ItemUnit>();
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();
    public DbSet<HsnSacCode> HsnSacCodes => Set<HsnSacCode>();
    public DbSet<Item> Items => Set<Item>();

    // Domain 3: Projects
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectPo> ProjectPos => Set<ProjectPo>();
    public DbSet<ProjectMilestone> ProjectMilestones => Set<ProjectMilestone>();
    public DbSet<ProjectExpense> ProjectExpenses => Set<ProjectExpense>();
    public DbSet<ProjectDelivery> ProjectDeliveries => Set<ProjectDelivery>();

    // Domain 4: Sales & Billing
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesInvoice> SalesInvoices => Set<SalesInvoice>();
    public DbSet<SalesInvoiceItem> SalesInvoiceItems => Set<SalesInvoiceItem>();
    public DbSet<CustomerReceipt> CustomerReceipts => Set<CustomerReceipt>();
    public DbSet<ReceiptAllocation> ReceiptAllocations => Set<ReceiptAllocation>();
    public DbSet<CreditDebitNote> CreditDebitNotes => Set<CreditDebitNote>();

    // Domain 5: Procurement & Payables
    public DbSet<PurchaseRequisition> PurchaseRequisitions => Set<PurchaseRequisition>();
    public DbSet<PurchaseRequisitionItem> PurchaseRequisitionItems => Set<PurchaseRequisitionItem>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<GoodsReceiptNote> GoodsReceiptNotes => Set<GoodsReceiptNote>();
    public DbSet<GrnItem> GrnItems => Set<GrnItem>();
    public DbSet<PurchaseBill> PurchaseBills => Set<PurchaseBill>();
    public DbSet<PurchaseBillItem> PurchaseBillItems => Set<PurchaseBillItem>();
    public DbSet<VendorPayment> VendorPayments => Set<VendorPayment>();
    public DbSet<VendorPaymentAllocation> VendorPaymentAllocations => Set<VendorPaymentAllocation>();

    // Domain 6: Office Inventory
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<StockIssue> StockIssues => Set<StockIssue>();

    // Domain 7: Accounting
    public DbSet<FinancialYear> FinancialYears => Set<FinancialYear>();
    public DbSet<AccountGroup> AccountGroups => Set<AccountGroup>();
    public DbSet<ChartOfAccount> ChartOfAccounts => Set<ChartOfAccount>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalLine> JournalLines => Set<JournalLine>();

    // Domain 8: Tax & GST
    public DbSet<GstTransaction> GstTransactions => Set<GstTransaction>();
    public DbSet<GstReconciliation> GstReconciliations => Set<GstReconciliation>();

    // Domain 9: Banking
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();
    public DbSet<BankReconciliation> BankReconciliations => Set<BankReconciliation>();
    public DbSet<PettyCashTransaction> PettyCashTransactions => Set<PettyCashTransaction>();

    // Domain 10: Payroll
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<SalaryStructure> SalaryStructures => Set<SalaryStructure>();
    public DbSet<PayrollRun> PayrollRuns => Set<PayrollRun>();
    public DbSet<PayrollItem> PayrollItems => Set<PayrollItem>();
    public DbSet<EmployeeAdvance> EmployeeAdvances => Set<EmployeeAdvance>();
    public DbSet<LeaveApplication> LeaveApplications => Set<LeaveApplication>();

    // Domain 11: Assets & Support
    public DbSet<FixedAsset> FixedAssets => Set<FixedAsset>();
    public DbSet<AssetAllocation> AssetAllocations => Set<AssetAllocation>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Security & Integrity Rule: Restrict Cascade Deletes across all foreign keys
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
''')

# Configurations
with open("src/SDK.ERP.Infrastructure/Configurations/CoreEntityConfigurations.cs", "w", encoding="utf-8") as f:
    f.write('''using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;
using SDK.ERP.Domain.Entities.Sales;
using SDK.ERP.Domain.Entities.Procurement;
using SDK.ERP.Domain.Entities.Inventory;
using SDK.ERP.Domain.Entities.Accounting;
using SDK.ERP.Domain.Entities.Tax;
using SDK.ERP.Domain.Entities.Banking;
using SDK.ERP.Domain.Entities.Payroll;
using SDK.ERP.Domain.Entities.AssetsAndSupport;

namespace SDK.ERP.Infrastructure.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies", "admin");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.CompanyCode).IsUnique();
        builder.Property(e => e.CompanyCode).HasMaxLength(20).IsRequired();
        builder.Property(e => e.CompanyName).HasMaxLength(150).IsRequired();
        builder.Property(e => e.LegalName).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Gstin).HasMaxLength(15);
        builder.Property(e => e.Pan).HasMaxLength(10).IsRequired();
        builder.Property(e => e.Tan).HasMaxLength(10);
        builder.Property(e => e.BaseCurrency).HasMaxLength(3).HasDefaultValue("INR");
    }
}

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches", "admin");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.BranchCode).IsUnique();
        builder.Property(e => e.BranchCode).HasMaxLength(30).IsRequired();
        builder.Property(e => e.BranchName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.StateCode).HasMaxLength(5).IsRequired();
        builder.Property(e => e.Gstin).HasMaxLength(15);
        builder.Property(e => e.AddressLine1).HasMaxLength(255).IsRequired();
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles", "admin");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.RoleName).IsUnique();
        builder.Property(e => e.RoleName).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(255);
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "admin");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Username).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.Username).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(100).IsRequired();
        builder.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
    }
}

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients", "master");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.ClientCode).IsUnique();
        builder.Property(e => e.ClientCode).HasMaxLength(30).IsRequired();
        builder.Property(e => e.ClientName).HasMaxLength(150).IsRequired();
        builder.Property(e => e.StateCode).HasMaxLength(5).IsRequired();
        builder.Property(e => e.Gstin).HasMaxLength(15);
        builder.Property(e => e.Pan).HasMaxLength(10);
        builder.Property(e => e.CreditLimit).HasColumnType("decimal(15,2)");
    }
}

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.ToTable("vendors", "master");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.VendorCode).IsUnique();
        builder.Property(e => e.VendorCode).HasMaxLength(30).IsRequired();
        builder.Property(e => e.VendorName).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Gstin).HasMaxLength(15);
        builder.Property(e => e.Pan).HasMaxLength(10).IsRequired();
        builder.Property(e => e.MsmeType).HasMaxLength(30);
        builder.Property(e => e.IfscCode).HasMaxLength(20);
    }
}

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("items", "master");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.ItemCode).IsUnique();
        builder.Property(e => e.ItemCode).HasMaxLength(30).IsRequired();
        builder.Property(e => e.ItemName).HasMaxLength(150).IsRequired();
        builder.Property(e => e.HsnSacCode).HasMaxLength(10);
        builder.Property(e => e.UnitCost).HasColumnType("decimal(12,2)");
        builder.Property(e => e.CurrentStockQty).HasColumnType("decimal(10,2)");
        builder.Property(e => e.ReorderLevelQty).HasColumnType("decimal(10,2)");
    }
}

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects", "project");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.ProjectCode).IsUnique();
        builder.Property(e => e.ProjectCode).HasMaxLength(50).IsRequired();
        builder.Property(e => e.ProjectName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.ContractValue).HasColumnType("decimal(15,2)");
        builder.Property(e => e.BudgetCost).HasColumnType("decimal(15,2)");
        builder.Property(e => e.Status).HasMaxLength(30);
    }
}

public class SalesInvoiceConfiguration : IEntityTypeConfiguration<SalesInvoice>
{
    public void Configure(EntityTypeBuilder<SalesInvoice> builder)
    {
        builder.ToTable("sales_invoices", "sales");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.InvoiceNumber).IsUnique();
        builder.HasIndex(e => new { e.ClientId, e.Status, e.InvoiceDate });
        builder.Property(e => e.InvoiceNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.PlaceOfSupply).HasMaxLength(5).IsRequired();
        builder.Property(e => e.TaxableAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.CgstAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.SgstAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.IgstAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.TotalInvoiceValue).HasColumnType("decimal(15,2)");
        builder.Property(e => e.PaidAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.OutstandingBalance).HasColumnType("decimal(15,2)");
        builder.Property(e => e.Status).HasMaxLength(20);
    }
}

public class PurchaseBillConfiguration : IEntityTypeConfiguration<PurchaseBill>
{
    public void Configure(EntityTypeBuilder<PurchaseBill> builder)
    {
        builder.ToTable("purchase_bills", "procurement");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.VendorId, e.Status, e.DueDate });
        builder.Property(e => e.VendorBillNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.PlaceOfSupply).HasMaxLength(5).IsRequired();
        builder.Property(e => e.TaxableAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.CgstAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.SgstAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.IgstAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.TotalBillAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.PaidAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.BalanceDue).HasColumnType("decimal(15,2)");
        builder.Property(e => e.Status).HasMaxLength(20);
    }
}

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("journal_entries", "accounting");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.VoucherNo).IsUnique();
        builder.Property(e => e.VoucherNo).HasMaxLength(50).IsRequired();
        builder.Property(e => e.VoucherType).HasMaxLength(30).IsRequired();
        builder.Property(e => e.TotalDebit).HasColumnType("decimal(15,2)");
        builder.Property(e => e.TotalCredit).HasColumnType("decimal(15,2)");
    }
}

public class JournalLineConfiguration : IEntityTypeConfiguration<JournalLine>
{
    public void Configure(EntityTypeBuilder<JournalLine> builder)
    {
        builder.ToTable("journal_lines", "accounting");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.AccountId, e.JournalEntryId });
        builder.Property(e => e.DebitAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.CreditAmount).HasColumnType("decimal(15,2)");
    }
}
''')

# SeedData.cs
with open("src/SDK.ERP.Infrastructure/Data/SeedData.cs", "w", encoding="utf-8") as f:
    f.write('''using Microsoft.EntityFrameworkCore;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Accounting;

namespace SDK.ERP.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        if (await context.Companies.AnyAsync()) return;

        // 1. Seed Company
        var company = new Company
        {
            CompanyCode = "SDK-CORP",
            CompanyName = "SDK Solutions Private Limited",
            LegalName = "SDK Solutions Private Limited",
            Gstin = "07AAAAA0000A1Z5",
            Pan = "AAAAA0000A",
            BaseCurrency = "INR",
            FinancialYearStart = new DateTime(2026, 4, 1),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // 2. Seed Head Office Branch
        var branch = new Branch
        {
            CompanyId = company.Id,
            BranchCode = "BR-HQ-DEL",
            BranchName = "Head Office - New Delhi",
            StateCode = "07",
            Gstin = "07AAAAA0000A1Z5",
            AddressLine1 = "Connaught Place, New Delhi",
            IsHeadOffice = true,
            IsActive = true
        };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        // 3. Seed System Roles
        var superAdminRole = new Role { RoleName = "SUPER_ADMIN", Description = "Full Enterprise Administration", IsSystemRole = true };
        var pmRole = new Role { RoleName = "PROJECT_MANAGER", Description = "Project Management & Delivery", IsSystemRole = true };
        var accountantRole = new Role { RoleName = "ACCOUNTANT", Description = "Financial Accounting & GST Compliance", IsSystemRole = true };
        var procurementRole = new Role { RoleName = "PROCUREMENT_OFFICER", Description = "Purchasing & Vendor Management", IsSystemRole = true };
        var storeKeeperRole = new Role { RoleName = "STORE_KEEPER", Description = "Internal Office Stock Consumables", IsSystemRole = true };

        context.Roles.AddRange(superAdminRole, pmRole, accountantRole, procurementRole, storeKeeperRole);
        await context.SaveChangesAsync();

        // 4. Seed Standard GST Tax Rates
        var taxRates = new List<TaxRate>
        {
            new TaxRate { TaxName = "GST 0% (Exempt)", RatePercentage = 0.00m, CgstPercentage = 0.00m, SgstPercentage = 0.00m, IgstPercentage = 0.00m },
            new TaxRate { TaxName = "GST 5%", RatePercentage = 5.00m, CgstPercentage = 2.50m, SgstPercentage = 2.50m, IgstPercentage = 5.00m },
            new TaxRate { TaxName = "GST 12%", RatePercentage = 12.00m, CgstPercentage = 6.00m, SgstPercentage = 6.00m, IgstPercentage = 12.00m },
            new TaxRate { TaxName = "GST 18%", RatePercentage = 18.00m, CgstPercentage = 9.00m, SgstPercentage = 9.00m, IgstPercentage = 18.00m },
            new TaxRate { TaxName = "GST 28%", RatePercentage = 28.00m, CgstPercentage = 14.00m, SgstPercentage = 14.00m, IgstPercentage = 28.00m }
        };
        context.TaxRates.AddRange(taxRates);
        await context.SaveChangesAsync();

        // 5. Seed Standard Units
        var units = new List<ItemUnit>
        {
            new ItemUnit { UnitCode = "NOS", UnitName = "Numbers", IsDecimalAllowed = false },
            new ItemUnit { UnitCode = "BOX", UnitName = "Boxes", IsDecimalAllowed = false },
            new ItemUnit { UnitCode = "PKT", UnitName = "Packets", IsDecimalAllowed = false },
            new ItemUnit { UnitCode = "HRS", UnitName = "Hours", IsDecimalAllowed = true },
            new ItemUnit { UnitCode = "KGS", UnitName = "Kilograms", IsDecimalAllowed = true }
        };
        context.ItemUnits.AddRange(units);
        await context.SaveChangesAsync();

        // 6. Seed Chart of Account Groups
        var assetGroup = new AccountGroup { GroupCode = "1000", GroupName = "Current Assets", AccountCategory = "ASSET" };
        var liabGroup = new AccountGroup { GroupCode = "2000", GroupName = "Current Liabilities", AccountCategory = "LIABILITY" };
        var equityGroup = new AccountGroup { GroupCode = "3000", GroupName = "Equity & Reserves", AccountCategory = "EQUITY" };
        var revGroup = new AccountGroup { GroupCode = "4000", GroupName = "Project Revenue", AccountCategory = "REVENUE" };
        var expGroup = new AccountGroup { GroupCode = "5000", GroupName = "Direct Expenses", AccountCategory = "EXPENSE" };

        context.AccountGroups.AddRange(assetGroup, liabGroup, equityGroup, revGroup, expGroup);
        await context.SaveChangesAsync();
    }
}
''')

print("ApplicationDbContext, Configurations and SeedData successfully generated.")
