namespace SDK.ERP.Domain.Entities.MasterData;

public class HsnSacCode
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "GOODS";
    public int? DefaultTaxRateId { get; set; }

    public TaxRate? DefaultTaxRate { get; set; }
}
