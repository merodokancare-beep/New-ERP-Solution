using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Procurement;

namespace SDK.ERP.Domain.Entities.AssetsAndSupport;

public class FixedAsset : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public string AssetTagCode { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public long? PurchaseBillId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal PurchaseCost { get; set; }
    public decimal CurrentBookValue { get; set; }
    public decimal DepreciationRate { get; set; } = 40.00m;
    public string Status { get; set; } = "AVAILABLE";

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public PurchaseBill? PurchaseBill { get; set; }

    public ICollection<AssetAllocation> Allocations { get; set; } = new List<AssetAllocation>();
}
