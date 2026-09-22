namespace SDK.ERP.Domain.Entities.Sales;

public class ReceiptAllocation
{
    public long Id { get; set; }
    public long ReceiptId { get; set; }
    public long InvoiceId { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal TdsDeductedByClient { get; set; }
    public decimal CashDiscountAllowed { get; set; }

    public CustomerReceipt Receipt { get; set; } = null!;
    public SalesInvoice Invoice { get; set; } = null!;
}
