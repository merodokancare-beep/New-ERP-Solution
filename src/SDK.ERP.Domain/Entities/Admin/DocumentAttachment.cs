using System.ComponentModel.DataAnnotations.Schema;

namespace SDK.ERP.Domain.Entities.Admin;

public class DocumentAttachment
{
    public long Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public long EntityId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string FileHashSha256 { get; set; } = string.Empty;
    public long? UploadedBy { get; set; }
    public long? UploaderId { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public int VersionNumber { get; set; } = 1;

    [ForeignKey(nameof(UploaderId))]
    public User? Uploader { get; set; }
}
