using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Accounting;

namespace SDK.ERP.Domain.Entities.Tax;

public class GstTransaction : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long? VoucherId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string? PartyGstin { get; set; }
    public string PlaceOfSupply { get; set; } = string.Empty;
    public string? HsnSacCode { get; set; }
    public decimal TaxableValue { get; set; }
    public decimal TaxRatePercentage { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public string ReturnPeriod { get; set; } = string.Empty;
    public bool IsFiled { get; set; }

    public Company Company { get; set; } = null!;
    
    [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(VoucherId))]
    public JournalEntry? Voucher { get; set; }
}
