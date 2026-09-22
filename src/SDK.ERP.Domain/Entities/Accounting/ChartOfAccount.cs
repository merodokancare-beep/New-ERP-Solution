using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Accounting;

public class ChartOfAccount
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public decimal OpeningBalance { get; set; }
    public string OpeningBalanceType { get; set; } = "DR";
    public decimal CurrentBalance { get; set; }
    public bool IsSystemAccount { get; set; }
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
    public AccountGroup Group { get; set; } = null!;
}
