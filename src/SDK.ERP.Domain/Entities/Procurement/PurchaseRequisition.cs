using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseRequisition
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string PrNumber { get; set; } = string.Empty;
    public long RequestedByUserId { get; set; }
    public int? DepartmentId { get; set; }
    public long? ProjectId { get; set; }
    public string PurposeType { get; set; } = "OFFICE_USE";
    public DateTime RequiredByDate { get; set; }
    public string Status { get; set; } = "DRAFT";

    public Company Company { get; set; } = null!;
    public User RequestedByUser { get; set; } = null!;
    public Project? Project { get; set; }

    public ICollection<PurchaseRequisitionItem> Items { get; set; } = new List<PurchaseRequisitionItem>();
}
