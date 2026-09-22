using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Sales;

public class SalesInvoiceItem
{
    public long Id { get; set; }
    public long InvoiceId { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public string HsnSacCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1.00m;
    public int? UnitId { get; set; }
    public decimal UnitRate { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxableValue { get; set; }
    public int TaxRateId { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal LineTotal { get; set; }

    public SalesInvoice Invoice { get; set; } = null!;
    public ItemUnit? Unit { get; set; }
    public TaxRate TaxRate { get; set; } = null!;
}
