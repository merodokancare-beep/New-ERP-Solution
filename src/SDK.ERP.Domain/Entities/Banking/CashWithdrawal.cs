using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Banking;

/// <summary>
/// Represents cash inflows / origins into the petty cash box / cash book.
/// Includes bank cash withdrawals as well as general cash receipts (customer receipts, refunds, advances returned).
/// </summary>
public class CashWithdrawal : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }

    /// <summary>
    /// Unique Reference Number (e.g. CW-2026-0001 or CR-2026-0001)
    /// </summary>
    public string ReferenceNumber { get; set; } = string.Empty;

    public DateTime WithdrawalDate { get; set; } = DateTime.Today;

    /// <summary>
    /// Source of cash inflow:
    /// BANK_WITHDRAWAL (Bank Cash Withdrawal)
    /// CUSTOMER_RECEIPT (Customer Cash Receipt)
    /// REFUND (Refund / Rebate)
    /// RETURNED_ADVANCE (Returned Staff Advance)
    /// OPENING_BALANCE (Imprest Float Opening Balance)
    /// OTHER (Other Cash Inflow)
    /// </summary>
    public string SourceType { get; set; } = "BANK_WITHDRAWAL";

    /// <summary>
    /// Associated corporate bank account if withdrawn from bank
    /// </summary>
    public long? BankAccountId { get; set; }
    public BankAccount? BankAccount { get; set; }

    /// <summary>
    /// Cheque Number, ATM slip reference, NEFT ref, or receipt voucher no
    /// </summary>
    public string? ChequeOrRefNo { get; set; }

    /// <summary>
    /// Total inflow amount withdrawn or received
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Cumulative amount utilised/allocated against cash expenses
    /// </summary>
    public decimal UtilisedAmount { get; set; }

    /// <summary>
    /// Custodian or employee who physically withdrew or received the cash
    /// </summary>
    public string WithdrawnBy { get; set; } = string.Empty;

    /// <summary>
    /// Purpose or business justification for the cash inflow / withdrawal
    /// </summary>
    public string Purpose { get; set; } = string.Empty;

    /// <summary>
    /// Approving officer / manager
    /// </summary>
    public string ApprovedBy { get; set; } = string.Empty;

    /// <summary>
    /// Link to the bank statement line so it can be reconciled
    /// </summary>
    public long? BankStatementLineId { get; set; }
    public BankTransaction? BankStatementLine { get; set; }

    /// <summary>
    /// Bank statement reference text, transaction ID or statement line date
    /// </summary>
    public string? BankStatementRef { get; set; }

    /// <summary>
    /// Status: OPEN (0 utilised) | PARTLY_UTILISED | FULLY_UTILISED | CLOSED
    /// </summary>
    public string Status { get; set; } = "OPEN";

    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation to allocation records
    public ICollection<CashAllocation> Allocations { get; set; } = new List<CashAllocation>();
}

