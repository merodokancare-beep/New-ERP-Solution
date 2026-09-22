using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Sales;

public class CustomerReceipt
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ClientId { get; set; }
    public long BankAccountId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public decimal AmountReceived { get; set; }
    public decimal UnallocatedAmount { get; set; }
    public string PaymentMode { get; set; } = "NEFT";
    public string? TransactionRefNo { get; set; }
    public string Status { get; set; } = "POSTED";
    public long? JournalEntryId { get; set; }

    public Company Company { get; set; } = null!;
    public Client Client { get; set; } = null!;

    public ICollection<ReceiptAllocation> Allocations { get; set; } = new List<ReceiptAllocation>();
}
