using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Accounting;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Banking;

public class PettyCashTransaction
{
    public long Id { get; set; }
    public long BankAccountId { get; set; }
    public long CustodianUserId { get; set; }
    public DateTime EntryDate { get; set; }
    public long ExpenseAccountId { get; set; }
    public long? ProjectId { get; set; }
    public decimal Amount { get; set; }
    public string PaidTo { get; set; } = string.Empty;
    public long? ReceiptDocId { get; set; }
    public string ApprovalStatus { get; set; } = "APPROVED";

    public BankAccount BankAccount { get; set; } = null!;
    public User CustodianUser { get; set; } = null!;
    public ChartOfAccount ExpenseAccount { get; set; } = null!;
    public Project? Project { get; set; }
    public DocumentAttachment? ReceiptDoc { get; set; }
}
