using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Procurement;

public class GrnItem
{
    public long Id { get; set; }
    public long GrnId { get; set; }
    public long PoItemId { get; set; }
    public long? ItemId { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal AcceptedQty { get; set; }
    public decimal RejectedQty { get; set; }
    public string? RejectionReason { get; set; }

    public GoodsReceiptNote GoodsReceiptNote { get; set; } = null!;
    public PurchaseOrderItem PoItem { get; set; } = null!;
    public Item? Item { get; set; }
}
