using SDK.ERP.Domain.Entities.Accounting;

namespace SDK.ERP.Domain.Entities.Banking;

public class BankTransaction
{
    public long Id { get; set; }
    public long BankAccountId { get; set; }
    public long? VoucherId { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime? ValueDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public bool IsReconciled { get; set; }

    public BankAccount BankAccount { get; set; } = null!;
    public JournalEntry? Voucher { get; set; }
}
