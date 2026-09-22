namespace SDK.ERP.Domain.Entities.Admin;

public class ApprovalRule
{
    public long Id { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int ApproverRoleId { get; set; }
    public int LevelOrder { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public Role ApproverRole { get; set; } = null!;
}
