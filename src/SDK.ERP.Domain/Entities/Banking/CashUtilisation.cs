using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Accounting;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Banking;

/// <summary>
/// Represents a petty cash payment / cash utilisation expense.
/// </summary>
public class CashUtilisation : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }

    /// <summary>
    /// Petty Cash Voucher Number (e.g. PCV-2026-0001)
    /// </summary>
    public string VoucherNumber { get; set; } = string.Empty;

    public DateTime ExpenseDate { get; set; } = DateTime.Today;

    /// <summary>
    /// Total expense amount paid out in cash
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Total amount allocated against cash withdrawals/inflows
    /// </summary>
    public decimal AllocatedAmount { get; set; }

    /// <summary>
    /// Recipient, vendor or person to whom cash was paid
    /// </summary>
    public string Payee { get; set; } = string.Empty;

    /// <summary>
    /// Expense category or ledger head name (e.g. Office Supplies, Travel & Conveyance, Food & Refreshments, Site Petty Cash, Minor Repairs, Courier)
    /// </summary>
    public string ExpenseCategory { get; set; } = string.Empty;

    /// <summary>
    /// General Ledger Head / Chart of Account link
    /// </summary>
    public long? ExpenseAccountId { get; set; }
    public ChartOfAccount? ExpenseAccount { get; set; }

    /// <summary>
    /// Cost centre or project link
    /// </summary>
    public long? ProjectId { get; set; }
    public Project? Project { get; set; }

    /// <summary>
    /// Cost Centre designation
    /// </summary>
    public string? CostCenter { get; set; }

    /// <summary>
    /// Purpose or description of the expense
    /// </summary>
    public string Purpose { get; set; } = string.Empty;

    /// <summary>
    /// Bill / receipt / cash memo attachment details
    /// </summary>
    public string? AttachmentFileName { get; set; }
    public string? AttachmentFilePath { get; set; }

    /// <summary>
    /// Custodian or person who disbursed the cash
    /// </summary>
    public string PaidBy { get; set; } = string.Empty;

    /// <summary>
    /// Approving officer / manager
    /// </summary>
    public string ApprovedBy { get; set; } = string.Empty;

    /// <summary>
    /// Status: DRAFT, APPROVED, POSTED
    /// </summary>
    public string Status { get; set; } = "DRAFT";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation to allocation records
    public ICollection<CashAllocation> Allocations { get; set; } = new List<CashAllocation>();
}

