using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Banking;

public class BankReconciliation
{
    public long Id { get; set; }
    public long BankAccountId { get; set; }
    public DateTime StatementDate { get; set; }
    public decimal ClosingBookBalance { get; set; }
    public decimal StatementBalance { get; set; }
    public decimal UnreconciledDifference { get; set; }
    public bool IsReconciled { get; set; }
    public long ReconciledBy { get; set; }

    public BankAccount BankAccount { get; set; } = null!;
    public User Reconciler { get; set; } = null!;
}
