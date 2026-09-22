using SDK.ERP.Domain.Common;

namespace SDK.ERP.Domain.Entities.Admin;

public class User : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public int RoleId { get; set; }
    public long? EmployeeId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public long? ReportingManagerId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // User Profile Enhancements
    public string? FullName { get; set; }
    public string? Designation { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; } // Personal profile avatar image URL path

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public User? ReportingManager { get; set; }
}
