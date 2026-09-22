namespace SDK.ERP.Domain.Entities.Projects;

public class ProjectMilestone
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string MilestoneName { get; set; } = string.Empty;
    public DateTime ExpectedDate { get; set; }
    public decimal MilestoneAmount { get; set; }
    public decimal? PercentageOfContract { get; set; }
    public string Status { get; set; } = "PENDING";
    public DateTime? CompletionDate { get; set; }

    public Project Project { get; set; } = null!;
}
