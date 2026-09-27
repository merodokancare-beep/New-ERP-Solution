using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Sales;

public class CustomerReceipt
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long? ClientId { get; set; }
    public long BankAccountId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public decimal AmountReceived { get; set; }
    public decimal UnallocatedAmount { get; set; }
    public string PaymentMode { get; set; } = "NEFT";
    public string? TransactionRefNo { get; set; }
    public string Status { get; set; } = "POSTED";
    public long? JournalEntryId { get; set; }
    public long? ProjectId { get; set; }
    public long? InvoiceId { get; set; }
    public bool IsAdvance { get; set; }
    public decimal TdsAmount { get; set; }
    public decimal GstTdsAmount { get; set; }
    public decimal SecurityDepositAmount { get; set; }
    public decimal OtherDeductionAmount { get; set; }
    public decimal NetAmountReceived { get; set; }
    public decimal TotalAmountReceived { get; set; }
    public string? ExpenseHead { get; set; }
    public string? Remarks { get; set; }
    public long? ReceiptDocId { get; set; }

    public Company Company { get; set; } = null!;
    public Client? Client { get; set; }
    public Projects.Project? Project { get; set; }
    public SalesInvoice? Invoice { get; set; }

    public ICollection<ReceiptAllocation> Allocations { get; set; } = new List<ReceiptAllocation>();
}
