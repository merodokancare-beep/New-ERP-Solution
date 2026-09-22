namespace SDK.ERP.Domain.Entities.Procurement;

public class VendorPaymentAllocation
{
    public long Id { get; set; }
    public long VendorPaymentId { get; set; }
    public long PurchaseBillId { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal TdsDeducted { get; set; }

    public VendorPayment Payment { get; set; } = null!;
    public PurchaseBill PurchaseBill { get; set; } = null!;
}
