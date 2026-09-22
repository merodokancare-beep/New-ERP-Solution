using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Payroll;

public class Employee : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string DesignationTitle { get; set; } = string.Empty;
    public long? ReportingManagerId { get; set; }
    public DateTime DateOfJoining { get; set; }
    public string Pan { get; set; } = string.Empty;
    public string? Uan { get; set; }
    public string BankAccountNo { get; set; } = string.Empty;
    public string BankIfsc { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Employee? ReportingManager { get; set; }

    public ICollection<SalaryStructure> SalaryStructures { get; set; } = new List<SalaryStructure>();
    public ICollection<LeaveApplication> LeaveApplications { get; set; } = new List<LeaveApplication>();
}
