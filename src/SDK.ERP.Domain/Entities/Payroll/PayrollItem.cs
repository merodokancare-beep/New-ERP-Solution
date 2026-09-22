namespace SDK.ERP.Domain.Entities.Payroll;

public class PayrollItem
{
    public long Id { get; set; }
    public long PayrollRunId { get; set; }
    public long EmployeeId { get; set; }
    public int WorkingDaysInMonth { get; set; } = 30;
    public decimal DaysPayable { get; set; } = 30.0m;
    public decimal GrossEarned { get; set; }
    public decimal PfDeduction { get; set; }
    public decimal PtDeduction { get; set; }
    public decimal TdsDeduction { get; set; }
    public decimal AdvanceDeduction { get; set; }
    public decimal NetSalary { get; set; }
    public string PaymentStatus { get; set; } = "UNPAID";

    public PayrollRun PayrollRun { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
}
