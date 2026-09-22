namespace SDK.ERP.Domain.Entities.Payroll;

public class SalaryStructure
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public decimal CtcAnnual { get; set; }
    public decimal GrossMonthly { get; set; }
    public decimal BasicPay { get; set; }
    public decimal Hra { get; set; }
    public decimal SpecialAllowance { get; set; }
    public decimal PfEmployee { get; set; }
    public decimal ProfessionalTax { get; set; }
    public bool IsActive { get; set; } = true;

    public Employee Employee { get; set; } = null!;
}
