using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Payroll;

public class PayrollRun
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string MonthYear { get; set; } = string.Empty;
    public int TotalEmployeesProcessed { get; set; }
    public decimal TotalGrossSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalNetPayable { get; set; }
    public string Status { get; set; } = "DRAFT";
    public long? JournalEntryId { get; set; }

    public Company Company { get; set; } = null!;
    public ICollection<PayrollItem> Items { get; set; } = new List<PayrollItem>();
}
