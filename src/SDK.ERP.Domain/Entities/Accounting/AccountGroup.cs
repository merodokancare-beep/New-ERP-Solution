namespace SDK.ERP.Domain.Entities.Accounting;

public class AccountGroup
{
    public int Id { get; set; }
    public string GroupCode { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string AccountCategory { get; set; } = string.Empty;
    public int? ParentGroupId { get; set; }

    public AccountGroup? ParentGroup { get; set; }
    public ICollection<AccountGroup> SubGroups { get; set; } = new List<AccountGroup>();
    public ICollection<ChartOfAccount> Accounts { get; set; } = new List<ChartOfAccount>();
}
