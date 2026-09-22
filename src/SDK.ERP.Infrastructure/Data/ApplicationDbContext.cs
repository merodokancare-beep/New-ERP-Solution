using Microsoft.EntityFrameworkCore;
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

using SDK.ERP.Application.Common;
using SDK.ERP.Domain.Common;

namespace SDK.ERP.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentTenantProvider? _tenantProvider;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentTenantProvider? tenantProvider = null) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public long ActiveTenantId => _tenantProvider?.CurrentCompanyId ?? 0;

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

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Zero Data Leakage: Register EF Core Global Query Filter on all tenant-scoped entities
        modelBuilder.Entity<Branch>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
        modelBuilder.Entity<User>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
        modelBuilder.Entity<Client>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
        modelBuilder.Entity<Vendor>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
        modelBuilder.Entity<Item>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
        modelBuilder.Entity<Project>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
        modelBuilder.Entity<SalesInvoice>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
        modelBuilder.Entity<PurchaseOrder>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
        modelBuilder.Entity<BankAccount>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
        modelBuilder.Entity<JournalEntry>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
        modelBuilder.Entity<GstTransaction>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
        modelBuilder.Entity<Employee>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
        modelBuilder.Entity<FixedAsset>().HasQueryFilter(e => _tenantProvider == null || _tenantProvider.CurrentCompanyId == 0 || e.CompanyId == _tenantProvider.CurrentCompanyId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Zero Data Leakage: Enforce tenant ownership on all inserts, updates, and deletes
        var currentTenantId = _tenantProvider?.CurrentCompanyId ?? 0;
        if (currentTenantId > 0)
        {
            foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.CompanyId == 0)
                    {
                        entry.Entity.CompanyId = currentTenantId;
                    }
                }
                else if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    if (entry.Entity.CompanyId != currentTenantId && entry.Entity.CompanyId != 0)
                    {
                        throw new InvalidOperationException($"Security Violation: Active organization {currentTenantId} is not permitted to mutate entity belonging to organization {entry.Entity.CompanyId}.");
                    }
                }
            }
        }

        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is not AuditLog && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
            .ToList();

        var auditEntries = new List<AuditLog>();
        foreach (var entry in entries)
        {
            var tableName = entry.Entity.GetType().Name;
            var actionType = entry.State.ToString().ToUpper();
            var timestamp = DateTime.UtcNow;
            var rawHashData = $"{tableName}|{actionType}|{timestamp:O}|{Guid.NewGuid()}";
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawHashData));
            var hashHex = Convert.ToHexString(hashBytes).ToLowerInvariant();

            auditEntries.Add(new AuditLog
            {
                TableName = tableName,
                ActionType = actionType,
                CurrentHash = hashHex,
                PreviousHash = "GENESIS_ROOT",
                CreatedAt = timestamp,
                IpAddress = "127.0.0.1",
                UserAgent = "SDK_ERP_APPLICATION_CORE"
            });
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        if (auditEntries.Any())
        {
            AuditLogs.AddRange(auditEntries);
            await base.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}
