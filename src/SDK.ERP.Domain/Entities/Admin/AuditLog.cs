namespace SDK.ERP.Domain.Entities.Admin;

public class AuditLog
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public long RecordId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string? OldPayload { get; set; }
    public string? NewPayload { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? PreviousHash { get; set; }
    public string? CurrentHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
