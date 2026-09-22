namespace SDK.ERP.Domain.Entities.MasterData;

public class ItemCategory
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    public ItemCategory? ParentCategory { get; set; }
    public ICollection<ItemCategory> SubCategories { get; set; } = new List<ItemCategory>();
}
