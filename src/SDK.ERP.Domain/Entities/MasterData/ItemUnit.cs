namespace SDK.ERP.Domain.Entities.MasterData;

public class ItemUnit
{
    public int Id { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public bool IsDecimalAllowed { get; set; }
}
