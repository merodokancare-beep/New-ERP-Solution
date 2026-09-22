using SDK.ERP.Domain.Common;

namespace SDK.ERP.Domain.Entities.Admin;

public class Branch : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string StateCode { get; set; } = string.Empty;
    public string? Gstin { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public bool IsHeadOffice { get; set; }
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
    public ICollection<User> Users { get; set; } = new List<User>();
}
