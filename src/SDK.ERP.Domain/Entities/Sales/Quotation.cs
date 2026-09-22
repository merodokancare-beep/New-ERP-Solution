using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Sales;

public class Quotation
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ClientId { get; set; }
    public long? ProjectId { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public DateTime QuoteDate { get; set; }
    public DateTime ValidityDate { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string Status { get; set; } = "DRAFT";

    public Company Company { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Project? Project { get; set; }
}
