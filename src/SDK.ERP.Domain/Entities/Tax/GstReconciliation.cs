using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Procurement;

namespace SDK.ERP.Domain.Entities.Tax;

public class GstReconciliation
{
    public long Id { get; set; }
    public string FinancialPeriod { get; set; } = string.Empty;
    public string PortalGstr2bInvoiceNo { get; set; } = string.Empty;
    public string PortalVendorGstin { get; set; } = string.Empty;
    public decimal PortalTaxableValue { get; set; }
    public decimal PortalTaxAmount { get; set; }
    public long? InternalPurchaseBillId { get; set; }
    public decimal? InternalTaxableValue { get; set; }
    public string MatchStatus { get; set; } = string.Empty;
    public string ItcEligibility { get; set; } = "ELIGIBLE";
    public long? ReconciledBy { get; set; }

    public PurchaseBill? InternalPurchaseBill { get; set; }
    public User? Reconciler { get; set; }
}
