namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseOrderLog
{
    public long Id { get; set; }
    public long PoId { get; set; }
    public long? PurchaseOrderId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public string? PaymentMode { get; set; }
    public string? VoucherOrRefNo { get; set; }
    public long? PerformedById { get; set; }
    public string PerformedByName { get; set; } = string.Empty;
    public string? PerformedByRole { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public PurchaseOrder PurchaseOrder { get; set; } = null!;
}
