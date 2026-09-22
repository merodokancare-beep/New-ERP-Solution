namespace SDK.ERP.Domain.Entities.Admin;

public class Company
{
    public long Id { get; set; }
    public string CompanyCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string? Gstin { get; set; }
    public string Pan { get; set; } = string.Empty;
    public string? Tan { get; set; }
    public string BaseCurrency { get; set; } = "INR";
    public DateTime FinancialYearStart { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Self-Service Branding & Identity
    public string? LogoUrl { get; set; }        // Corporate brand logo URL path e.g. "/uploads/tenants/org_1/branding/logo.png"
    public string? BrandShortName { get; set; } // Short acronym for logo box e.g. "SDK", "ACME"
    public string? Tagline { get; set; }        // Header brand title e.g. "SOLUTIONS ERP"
    public string? Industry { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? StateCode { get; set; }
    public string? Pincode { get; set; }
    public string? Cin { get; set; }
    public string? MsmeUdyamNo { get; set; }

    // Invoicing & Banking Coordinates
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankIfsc { get; set; }
    public string? BankBranch { get; set; }
    public string? UpiId { get; set; }
    public string? AuthorizedSignatoryName { get; set; }
    public string? AuthorizedSignatoryDesignation { get; set; }
    public string? TermsAndConditions { get; set; }

    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
