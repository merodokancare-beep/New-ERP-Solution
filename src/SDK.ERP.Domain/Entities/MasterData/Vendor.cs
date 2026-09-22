using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.MasterData;

public class Vendor : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string VendorCode { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string VendorCategory { get; set; } = "GENERAL";
    public string? Gstin { get; set; }
    public string Pan { get; set; } = string.Empty;
    public string? MsmeType { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNo { get; set; }
    public string? IfscCode { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
}
