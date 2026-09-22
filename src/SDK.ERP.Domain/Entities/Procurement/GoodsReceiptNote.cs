using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Procurement;

public class GoodsReceiptNote
{
    public long Id { get; set; }
    public long PoId { get; set; }
    public string GrnNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public string? VendorDcNumber { get; set; }
    public long ReceivedByUserId { get; set; }
    public string InspectionStatus { get; set; } = "ACCEPTED";

    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public User ReceivedByUser { get; set; } = null!;

    public ICollection<GrnItem> Items { get; set; } = new List<GrnItem>();
}
