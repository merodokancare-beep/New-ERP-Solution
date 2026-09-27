namespace SDK.ERP.Domain.Entities.MasterData;

public class ExpenseType
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsProjectRelated { get; set; } = false;
    public bool ReceiveDirectPayments { get; set; } = false;
    public decimal? OpeningAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
