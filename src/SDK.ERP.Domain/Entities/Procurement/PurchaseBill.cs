using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseBill
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long VendorId { get; set; }
    public long? PoId { get; set; }
    public long? ProjectId { get; set; }
    public string VendorBillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public DateTime DueDate { get; set; }
    public string PlaceOfSupply { get; set; } = string.Empty;
    public decimal TaxableAmount { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal TotalBillAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceDue { get; set; }
    public string Status { get; set; } = "UNPAID";
    public long? JournalEntryId { get; set; }

    public Company Company { get; set; } = null!;
    public Vendor Vendor { get; set; } = null!;
    public PurchaseOrder? PurchaseOrder { get; set; }
    public Project? Project { get; set; }

    public ICollection<PurchaseBillItem> Items { get; set; } = new List<PurchaseBillItem>();
    public ICollection<VendorPaymentAllocation> Allocations { get; set; } = new List<VendorPaymentAllocation>();
}
