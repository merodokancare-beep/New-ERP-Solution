using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Payroll;

namespace SDK.ERP.Domain.Entities.AssetsAndSupport;

public class AssetAllocation
{
    public long Id { get; set; }
    public long AssetId { get; set; }
    public long AllocatedToEmployeeId { get; set; }
    public DateTime AllocatedDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string? HandoverCondition { get; set; }
    public long AllocatedBy { get; set; }

    public FixedAsset Asset { get; set; } = null!;
    public Employee AllocatedToEmployee { get; set; } = null!;
    public User AllocatedByUser { get; set; } = null!;
}
