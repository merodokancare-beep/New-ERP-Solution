using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseOrder : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public long VendorId { get; set; }
    public long? ProjectId { get; set; }
    public long? PrId { get; set; }
    public string PoNumber { get; set; } = string.Empty;
    public string? PoType { get; set; }
    public DateTime PoDate { get; set; }
    public DateTime? DeliveryDueDate { get; set; }
    public string? ShipTo { get; set; }
    public string? Remarks { get; set; }
    public string? TermsConditions { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal GstAmount { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal TotalPoValue { get; set; }
    public string ApprovalStatus { get; set; } = "APPROVED";
    public long? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public long? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public string? CreatedByRole { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? MarkedForPaymentAt { get; set; }

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Vendor Vendor { get; set; } = null!;
    public Project? Project { get; set; }
    public PurchaseRequisition? Requisition { get; set; }
    public User? Approver { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    public ICollection<GoodsReceiptNote> GoodsReceiptNotes { get; set; } = new List<GoodsReceiptNote>();
}
