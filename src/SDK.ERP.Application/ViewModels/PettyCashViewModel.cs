using SDK.ERP.Domain.Entities.Accounting;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Banking;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Application.ViewModels;

public class PettyCashViewModel
{
    public string ActiveTab { get; set; } = "overview";

    public List<CashWithdrawal> Withdrawals { get; set; } = new();
    public List<CashUtilisation> Utilisations { get; set; } = new();
    public List<CashAllocation> Allocations { get; set; } = new();
    public List<BankAccount> BankAccounts { get; set; } = new();
    public List<Project> Projects { get; set; } = new();
    public List<ExpenseType> ExpenseTypes { get; set; } = new();
    public List<BankTransaction> BankTransactions { get; set; } = new();
    public List<User> Users { get; set; } = new();

    // Summary KPIs
    public decimal TotalCashInflow { get; set; }
    public decimal TotalCashUtilised { get; set; }
    public decimal CashInHand => TotalCashInflow - TotalCashUtilised;
    public decimal TotalAllocated { get; set; }
    public decimal UnallocatedUtilisation => Math.Max(0, TotalCashUtilised - TotalAllocated);
    public decimal UnutilisedInflow => Math.Max(0, TotalCashInflow - TotalAllocated);

    public int OpenWithdrawalsCount { get; set; }
    public int PendingUtilisationsCount { get; set; }
}

public class CashWithdrawalInputModel
{
    public DateTime WithdrawalDate { get; set; } = DateTime.Today;
    public string SourceType { get; set; } = "BANK_WITHDRAWAL";
    public long? BankAccountId { get; set; }
    public string? ChequeOrRefNo { get; set; }
    public decimal Amount { get; set; }
    public string WithdrawnBy { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
    public long? BankStatementLineId { get; set; }
    public string? BankStatementRef { get; set; }
    public string? Notes { get; set; }
}

public class CashUtilisationInputModel
{
    public DateTime ExpenseDate { get; set; } = DateTime.Today;
    public decimal Amount { get; set; }
    public string Payee { get; set; } = string.Empty;
    public string ExpenseCategory { get; set; } = string.Empty;
    public long? ExpenseAccountId { get; set; }
    public long? ProjectId { get; set; }
    public string? CostCenter { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string PaidBy { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
    public string Status { get; set; } = "APPROVED"; // Draft, Approved, Posted
    public bool AutoAllocateFifo { get; set; } = true;
}

public class ManualAllocationItem
{
    public long WithdrawalId { get; set; }
    public decimal Amount { get; set; }
}

