using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Inventory;

public class StockTransaction
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ItemId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public long ReferenceId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal BalanceQtyAfter { get; set; }

    public Company Company { get; set; } = null!;
    public Item Item { get; set; } = null!;
}
