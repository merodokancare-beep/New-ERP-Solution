using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.MasterData;

public class Client : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string ClientCode { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string BillingAddress { get; set; } = string.Empty;
    public string? ShippingAddress { get; set; }
    public string StateCode { get; set; } = string.Empty;
    public string? Gstin { get; set; }
    public string? Pan { get; set; }
    public decimal CreditLimit { get; set; }
    public int CreditDays { get; set; } = 30;
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
}
