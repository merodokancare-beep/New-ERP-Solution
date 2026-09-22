using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Accounting;

namespace SDK.ERP.Domain.Entities.Banking;

public class BankAccount : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long GlAccountId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string? BranchName { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string IfscCode { get; set; } = string.Empty;
    public string AccountType { get; set; } = "CURRENT";
    public decimal BookBalance { get; set; }
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
    public ChartOfAccount GlAccount { get; set; } = null!;
}
