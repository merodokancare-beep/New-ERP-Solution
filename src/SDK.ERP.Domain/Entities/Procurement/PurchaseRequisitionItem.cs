using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseRequisitionItem
{
    public long Id { get; set; }
    public long PrId { get; set; }
    public long? ItemId { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public decimal RequestedQty { get; set; }
    public int UnitId { get; set; }
    public decimal? EstimatedCost { get; set; }

    public PurchaseRequisition Requisition { get; set; } = null!;
    public Item? Item { get; set; }
    public ItemUnit Unit { get; set; } = null!;
}
