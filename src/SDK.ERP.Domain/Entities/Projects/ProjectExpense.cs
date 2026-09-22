using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Projects;

public class ProjectExpense
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public long ExpenseHeadId { get; set; }
    public long IncurredByUserId { get; set; }
    public DateTime ExpenseDate { get; set; }
    public decimal Amount { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal GstAmount { get; set; }
    public string PaymentMode { get; set; } = "REIMBURSEMENT";
    public string Status { get; set; } = "SUBMITTED";
    public long? ApprovedBy { get; set; }
    public long? ReceiptDocId { get; set; }

    public Project Project { get; set; } = null!;
    public User IncurredByUser { get; set; } = null!;
    public User? Approver { get; set; }
    public DocumentAttachment? ReceiptDoc { get; set; }
}
