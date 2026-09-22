using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseBillItem
{
    public long Id { get; set; }
    public long PurchaseBillId { get; set; }
    public long ExpenseAccountId { get; set; }
    public long? ItemId { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public string? HsnSacCode { get; set; }
    public decimal TaxableAmount { get; set; }
    public int TaxRateId { get; set; }
    public decimal GstAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public PurchaseBill PurchaseBill { get; set; } = null!;
    public Item? Item { get; set; }
    public TaxRate TaxRate { get; set; } = null!;
}
