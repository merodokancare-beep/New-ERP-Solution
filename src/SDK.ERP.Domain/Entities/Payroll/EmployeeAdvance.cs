namespace SDK.ERP.Domain.Entities.Payroll;

public class EmployeeAdvance
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public decimal AdvanceAmount { get; set; }
    public DateTime DisbursementDate { get; set; }
    public decimal MonthlyRecoveryAmount { get; set; }
    public decimal RecoveredAmount { get; set; }
    public decimal BalanceDue { get; set; }
    public string Status { get; set; } = "ACTIVE";

    public Employee Employee { get; set; } = null!;
}
