namespace SDK.ERP.Domain.Entities.Admin;

public class ApprovalRequest
{
    public long Id { get; set; }
    public long RuleId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public long EntityId { get; set; }
    public long RequestedBy { get; set; }
    public int AssignedToRoleId { get; set; }
    public string Status { get; set; } = "PENDING";
    public long? ActionBy { get; set; }
    public DateTime? ActionAt { get; set; }
    public string? Comments { get; set; }

    public ApprovalRule Rule { get; set; } = null!;
    public User Requester { get; set; } = null!;
    public Role AssignedRole { get; set; } = null!;
    public User? ActionUser { get; set; }
}
