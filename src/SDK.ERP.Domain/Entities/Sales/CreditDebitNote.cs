using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Sales;

public class CreditDebitNote
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string NoteType { get; set; } = string.Empty;
    public string PartyType { get; set; } = string.Empty;
    public long? ClientId { get; set; }
    public long? VendorId { get; set; }
    public long? OriginalInvoiceId { get; set; }
    public long? OriginalPurchaseBillId { get; set; }
    public string NoteNumber { get; set; } = string.Empty;
    public DateTime NoteDate { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal GstAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public long? JournalEntryId { get; set; }

    public Company Company { get; set; } = null!;
    public Client? Client { get; set; }
    public Vendor? Vendor { get; set; }
    public SalesInvoice? OriginalInvoice { get; set; }
}
