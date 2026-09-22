using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Projects;

public class ProjectPo
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string ClientPoNumber { get; set; } = string.Empty;
    public DateTime PoDate { get; set; }
    public decimal PoValue { get; set; }
    public DateTime? ValidityEndDate { get; set; }
    public string? ScopeOfWork { get; set; }
    public long? AttachmentDocId { get; set; }

    public Project Project { get; set; } = null!;
    public DocumentAttachment? Attachment { get; set; }
}
