using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.MasterData;

public class Item : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int UnitId { get; set; }
    public string? HsnSacCode { get; set; }
    public int TaxRateId { get; set; }
    public decimal UnitCost { get; set; }
    public decimal CurrentStockQty { get; set; }
    public decimal ReorderLevelQty { get; set; }
    public bool IsDurableAsset { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
    public ItemCategory Category { get; set; } = null!;
    public ItemUnit Unit { get; set; } = null!;
    public TaxRate TaxRate { get; set; } = null!;
}
