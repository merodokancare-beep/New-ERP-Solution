using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Accounting;

/// <summary>
/// Represents a Direct Payment / Expense voucher.
/// Can be released directly against a Project, directly to a Vendor, or against general Expense Heads.
/// Supports a complete approval and settlement lifecycle: PENDING -> UNSETTLED -> SETTLED (or DECLINED).
/// </summary>
public class DirectExpense : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }

    /// <summary>
    /// Expense Head Name (e.g. Project, Tax Payment, Office Expense, Business Development)
    /// </summary>
    public string ExpenseHead { get; set; } = string.Empty;
    public long? ExpenseHeadId { get; set; }

    /// <summary>
    /// Indicates if payment is directly to a vendor
    /// </summary>
    public bool IsVendorApplicable { get; set; }
    public long? VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    /// <summary>
    /// Selected General Ledger account (e.g. GST Payment, Electricity Expenses, ESIC, etc.)
    /// </summary>
    public long? LedgerId { get; set; }
    public string LedgerName { get; set; } = string.Empty;

    public DateTime PaymentDate { get; set; } = DateTime.Today;
    public decimal Amount { get; set; }

    /// <summary>
    /// Indicates if related to an active pipeline project
    /// </summary>
    public bool IsPipelineProject { get; set; }
    public long? ProjectId { get; set; }
    public Project? Project { get; set; }

    public string Purpose { get; set; } = string.Empty;
    public string? Remarks { get; set; }

    /// <summary>
    /// Lifecycle Status: PENDING (awaiting approval) | UNSETTLED (approved, awaiting payment) | SETTLED (paid/disbursed) | DECLINED (rejected)
    /// </summary>
    public string Status { get; set; } = "PENDING";

    // Creator audit
    public long? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Approval stage
    public long? ApprovedByUserId { get; set; }
    public string? ApprovedByUserName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalRemarks { get; set; }

    // Decline stage
    public long? DeclinedByUserId { get; set; }
    public string? DeclinedByUserName { get; set; }
    public DateTime? DeclinedAt { get; set; }
    public string? DeclineReason { get; set; }

    // Settlement stage
    public long? SettledByUserId { get; set; }
    public string? SettledByUserName { get; set; }
    public DateTime? SettledAt { get; set; }
    public string? SettlementPaymentMode { get; set; } // Cash, NetBanking, NEFT, Cheque, RTGS, UPI
    public string? SettlementReference { get; set; }   // UTR / Cheque / Transaction Ref
    public long? SettlementBankAccountId { get; set; }
    public string? SettlementBankName { get; set; }
    public string? SettlementNotes { get; set; }

    public long? JournalEntryId { get; set; }
}
