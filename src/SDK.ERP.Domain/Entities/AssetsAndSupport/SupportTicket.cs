using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.AssetsAndSupport;

public class SupportTicket
{
    public long Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public long RaisedByUserId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = "MEDIUM";
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long? AssignedToUserId { get; set; }
    public string Status { get; set; } = "OPEN";
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public User RaisedByUser { get; set; } = null!;
    public User? AssignedToUser { get; set; }
}
