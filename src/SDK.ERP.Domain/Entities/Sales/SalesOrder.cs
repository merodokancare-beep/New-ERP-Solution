using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Sales;

public class SalesOrder
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ClientId { get; set; }
    public long ProjectId { get; set; }
    public long? QuotationId { get; set; }
    public string SoNumber { get; set; } = string.Empty;
    public DateTime SoDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "CONFIRMED";

    public Company Company { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public Quotation? Quotation { get; set; }
}
