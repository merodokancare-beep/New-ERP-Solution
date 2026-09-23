using System.ComponentModel.DataAnnotations.Schema;
using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Projects;

public class ProjectDelivery
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string DcNumber { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public string? DispatchMode { get; set; }
    public string? TrackingRefNo { get; set; }
    public string? RecipientName { get; set; }
    public string Status { get; set; } = "DISPATCHED";
    public string? MaterialSummary { get; set; }
    public long? AttachmentDocId { get; set; }
    public long? AttachmentId { get; set; }

    public Project Project { get; set; } = null!;
    
    [ForeignKey(nameof(AttachmentDocId))]
    public DocumentAttachment? Attachment { get; set; }
}
