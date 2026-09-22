using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Inventory;

public class StockIssue
{
    public long Id { get; set; }
    public string IssueVoucherNo { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public long ItemId { get; set; }
    public decimal QuantityIssued { get; set; }
    public long? IssuedToEmployeeId { get; set; }
    public int? DepartmentId { get; set; }
    public string PurposeRemarks { get; set; } = "OFFICE_USE";
    public long ApprovedBy { get; set; }

    public Item Item { get; set; } = null!;
    public User Approver { get; set; } = null!;
}
