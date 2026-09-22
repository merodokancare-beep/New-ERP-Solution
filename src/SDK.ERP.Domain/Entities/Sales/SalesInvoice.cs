using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Sales;

public class SalesInvoice : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public long ClientId { get; set; }
    public long ProjectId { get; set; }
    public long? SalesOrderId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string PlaceOfSupply { get; set; } = string.Empty;
    public bool IsReverseCharge { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal TotalInvoiceValue { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OutstandingBalance { get; set; }
    public string Status { get; set; } = "DRAFT";
    public string? IrnNumber { get; set; }
    public string? QrCodePayload { get; set; }
    public long? JournalEntryId { get; set; }

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public SalesOrder? SalesOrder { get; set; }

    public ICollection<SalesInvoiceItem> Items { get; set; } = new List<SalesInvoiceItem>();
    public ICollection<ReceiptAllocation> Allocations { get; set; } = new List<ReceiptAllocation>();
}
