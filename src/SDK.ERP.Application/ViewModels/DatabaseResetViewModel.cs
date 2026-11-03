namespace SDK.ERP.Application.ViewModels;

public class DatabaseResetViewModel
{
    public bool IsSuperAdmin { get; set; }
    public string CurrentUsername { get; set; } = string.Empty;
    public string CurrentCompanyName { get; set; } = string.Empty;
    public long CurrentCompanyId { get; set; }

    // Master Data Statistics (100% Preserved)
    public int CompaniesCount { get; set; }
    public int BranchesCount { get; set; }
    public int UsersCount { get; set; }
    public int RolesCount { get; set; }
    public int ClientsCount { get; set; }
    public int VendorsCount { get; set; }
    public int ItemsCount { get; set; }
    public int ItemCategoriesCount { get; set; }
    public int ItemUnitsCount { get; set; }
    public int TaxRatesCount { get; set; }
    public int HsnSacCodesCount { get; set; }
    public int ProjectTypesCount { get; set; }
    public int ExpenseTypesCount { get; set; }
    public int FinancialYearsCount { get; set; }
    public int ChartOfAccountsCount { get; set; }
    public int BankAccountsCount { get; set; }
    public int EmployeesCount { get; set; }
    public int SalaryStructuresCount { get; set; }
    public int ApprovalRulesCount { get; set; }

    public int TotalMasterRecords =>
        CompaniesCount + BranchesCount + UsersCount + RolesCount + ClientsCount +
        VendorsCount + ItemsCount + ItemCategoriesCount + ItemUnitsCount + TaxRatesCount +
        HsnSacCodesCount + ProjectTypesCount + ExpenseTypesCount + FinancialYearsCount +
        ChartOfAccountsCount + BankAccountsCount + EmployeesCount + SalaryStructuresCount + ApprovalRulesCount;

    // Transactional Data Statistics (Wiped / Reset)
    public int PettyCashInflowsCount { get; set; }
    public int PettyCashExpensesCount { get; set; }
    public int PettyCashAllocationsCount { get; set; }
    public int DirectExpensesCount { get; set; }
    public int BankTransactionsCount { get; set; }
    public int BankReconciliationsCount { get; set; }
    public int SalesInvoicesCount { get; set; }
    public int SalesOrdersCount { get; set; }
    public int QuotationsCount { get; set; }
    public int CustomerReceiptsCount { get; set; }
    public int PurchaseOrdersCount { get; set; }
    public int PurchaseBillsCount { get; set; }
    public int GoodsReceiptNotesCount { get; set; }
    public int VendorPaymentsCount { get; set; }
    public int StockTransactionsCount { get; set; }
    public int JournalEntriesCount { get; set; }
    public int ProjectsCount { get; set; }
    public int PayrollRunsCount { get; set; }
    public int DocumentAttachmentsCount { get; set; }
    public int ApprovalRequestsCount { get; set; }

    public int TotalTransactionalRecords =>
        PettyCashInflowsCount + PettyCashExpensesCount + PettyCashAllocationsCount +
        DirectExpensesCount + BankTransactionsCount + BankReconciliationsCount +
        SalesInvoicesCount + SalesOrdersCount + QuotationsCount + CustomerReceiptsCount +
        PurchaseOrdersCount + PurchaseBillsCount + GoodsReceiptNotesCount + VendorPaymentsCount +
        StockTransactionsCount + JournalEntriesCount + ProjectsCount + PayrollRunsCount +
        DocumentAttachmentsCount + ApprovalRequestsCount;
}

public class ExecuteResetRequestModel
{
    public string Password { get; set; } = string.Empty;
    public string ConfirmationPhrase { get; set; } = string.Empty;
    public string Scope { get; set; } = "CURRENT_COMPANY"; // "CURRENT_COMPANY" or "ALL_COMPANIES"
    public bool ResetProjects { get; set; } = true;
    public bool ReseedSampleTransactions { get; set; } = false;
}

