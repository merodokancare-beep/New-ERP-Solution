using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Accounting;

public class FinancialYear
{
    public int Id { get; set; }
    public long CompanyId { get; set; }
    public string FyCode { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }
    public long? ClosedBy { get; set; }

    public Company Company { get; set; } = null!;
}
