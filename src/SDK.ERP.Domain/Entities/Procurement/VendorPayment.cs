using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Procurement;

public class VendorPayment
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long VendorId { get; set; }
    public long BankAccountId { get; set; }
    public string PaymentVoucherNo { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal TotalAmountPaid { get; set; }
    public string PaymentMode { get; set; } = "NEFT";
    public string? ChequeUtrNo { get; set; }
    public string Status { get; set; } = "PROCESSED";
    public long? JournalEntryId { get; set; }

    public Company Company { get; set; } = null!;
    public Vendor Vendor { get; set; } = null!;

    public ICollection<VendorPaymentAllocation> Allocations { get; set; } = new List<VendorPaymentAllocation>();
}
