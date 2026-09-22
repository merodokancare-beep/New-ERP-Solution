namespace SDK.ERP.Domain.Entities.Admin;

public class RolePermission
{
    public long Id { get; set; }
    public int RoleId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string SubmoduleCode { get; set; } = string.Empty;
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanApprove { get; set; }
    public bool CanPost { get; set; }
    public bool CanExport { get; set; }

    public Role Role { get; set; } = null!;
}
