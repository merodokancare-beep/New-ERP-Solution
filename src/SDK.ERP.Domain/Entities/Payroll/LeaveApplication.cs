using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Payroll;

public class LeaveApplication
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDING";
    public long? ApprovedBy { get; set; }

    public Employee Employee { get; set; } = null!;
    public User? Approver { get; set; }
}
