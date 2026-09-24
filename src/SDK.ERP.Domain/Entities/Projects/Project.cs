using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Projects;

public class Project : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public long ClientId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectType { get; set; } = "STANDARD";
    public long? ManagerId { get; set; }
    public decimal ContractValue { get; set; }
    public decimal BudgetCost { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? ExpectedEndDate { get; set; }
    public DateTime? ActualClosedDate { get; set; }
    public string Status { get; set; } = "PIPELINE";
    public long? ClosureApprovedBy { get; set; }
    public string? ClosureNotes { get; set; }
    public string? Description { get; set; }
    public string? Division { get; set; }
    public string? PhysicalFileStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public User? Manager { get; set; }
    public User? ClosureApprover { get; set; }

    public ICollection<ProjectPo> PurchaseOrders { get; set; } = new List<ProjectPo>();
    public ICollection<ProjectMilestone> Milestones { get; set; } = new List<ProjectMilestone>();
    public ICollection<ProjectExpense> Expenses { get; set; } = new List<ProjectExpense>();
    public ICollection<ProjectDelivery> Deliveries { get; set; } = new List<ProjectDelivery>();
}
