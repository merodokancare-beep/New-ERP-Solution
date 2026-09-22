using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseOrderItem
{
    public long Id { get; set; }
    public long PoId { get; set; }
    public long? ItemId { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public string? HsnSacCode { get; set; }
    public decimal OrderedQty { get; set; }
    public decimal ReceivedQty { get; set; }
    public int UnitId { get; set; }
    public decimal UnitRate { get; set; }
    public int TaxRateId { get; set; }
    public decimal LineTotal { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Item? Item { get; set; }
    public ItemUnit Unit { get; set; } = null!;
    public TaxRate TaxRate { get; set; } = null!;
}
