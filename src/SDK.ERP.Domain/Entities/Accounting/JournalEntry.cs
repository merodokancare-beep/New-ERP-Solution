using SDK.ERP.Domain.Common;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Accounting;

public class JournalEntry : ITenantEntity
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public int FyId { get; set; }
    public string VoucherNo { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public string VoucherType { get; set; } = string.Empty;
    public string? SourceEntityType { get; set; }
    public long? SourceEntityId { get; set; }
    public long? ProjectId { get; set; }
    public string Narration { get; set; } = string.Empty;
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public bool IsBalanced { get; set; } = true;
    public bool IsReversal { get; set; }
    public long CreatedBy { get; set; }

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    
    [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(FyId))]
    public FinancialYear FinancialYear { get; set; } = null!;
    
    public Project? Project { get; set; }
    
    [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(CreatedBy))]
    public User Creator { get; set; } = null!;

    public ICollection<JournalLine> Lines { get; set; } = new List<JournalLine>();
}
