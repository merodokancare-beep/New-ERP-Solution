namespace SDK.ERP.Domain.Entities.Admin;

/// <summary>
/// Represents a granular privilege explicitly granted or revoked for a specific user.
/// Overrides default role permissions, giving Admins centralized, dynamic control without hardcoding.
/// </summary>
public class UserPrivilege
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;       // e.g. "PROCUREMENT", "EXPENSES", "SALES", "PROJECTS"
    public string PrivilegeCode { get; set; } = string.Empty;    // e.g. "APPROVE_PAYMENT", "MAKE_PAYMENT", "VIEW"
    public bool IsGranted { get; set; } = true;
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public long? GrantedByUserId { get; set; }

    public User User { get; set; } = null!;
}
