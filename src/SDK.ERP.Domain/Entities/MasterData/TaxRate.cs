namespace SDK.ERP.Domain.Entities.MasterData;

public class TaxRate
{
    public int Id { get; set; }
    public string TaxName { get; set; } = string.Empty;
    public decimal RatePercentage { get; set; }
    public decimal CgstPercentage { get; set; }
    public decimal SgstPercentage { get; set; }
    public decimal IgstPercentage { get; set; }
    public bool IsActive { get; set; } = true;
}
