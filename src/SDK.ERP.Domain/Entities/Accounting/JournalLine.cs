using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Accounting;

public class JournalLine
{
    public long Id { get; set; }
    public long JournalEntryId { get; set; }
    public long AccountId { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public long? ClientId { get; set; }
    public long? VendorId { get; set; }
    public string? LineNarration { get; set; }

    public JournalEntry JournalEntry { get; set; } = null!;
    public ChartOfAccount Account { get; set; } = null!;
    public Client? Client { get; set; }
    public Vendor? Vendor { get; set; }
}
