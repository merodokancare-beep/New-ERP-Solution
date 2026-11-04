namespace SDK.ERP.Domain.Entities.MasterData;

public class StateMaster
{
    public int Id { get; set; }
    public string StateCode { get; set; } = string.Empty;
    public string StateName { get; set; } = string.Empty;
    public string? StateType { get; set; }
    public bool IsActive { get; set; } = true;
}

