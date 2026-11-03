using SDK.ERP.Domain.Common;

namespace SDK.ERP.Domain.Entities.Banking;

/// <summary>
/// Allocation table linking Cash Inflows / Withdrawals with Cash Utilisations.
/// Answers "where did this cash come from?".
/// Supports M:N mapping: one expense can draw on multiple withdrawals, and one withdrawal funds many expenses.
/// </summary>
public class CashAllocation : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }

    /// <summary>
    /// Foreign key to CashWithdrawal (the cash origin)
    /// </summary>
    public long WithdrawalId { get; set; }
    public CashWithdrawal Withdrawal { get; set; } = null!;

    /// <summary>
    /// Foreign key to CashUtilisation (the expense)
    /// </summary>
    public long UtilisationId { get; set; }
    public CashUtilisation Utilisation { get; set; } = null!;

    /// <summary>
    /// Allocated amount drawn from this withdrawal for this utilisation
    /// </summary>
    public decimal Amount { get; set; }

    public DateTime AllocationDate { get; set; } = DateTime.Today;

    /// <summary>
    /// User or process that performed the allocation
    /// </summary>
    public string? AllocatedBy { get; set; }

    /// <summary>
    /// True if manually chosen by the user; False if allocated via FIFO auto-allocation
    /// </summary>
    public bool IsManualOverride { get; set; }

    public string? Notes { get; set; }
}

