using System.ComponentModel.DataAnnotations;
using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Application.ViewModels;

public class LoginViewModel
{
    [Display(Name = "Organization / Company")]
    public long? SelectedCompanyId { get; set; }

    [Display(Name = "Organization Code")]
    public string? CompanyCode { get; set; }

    [Required(ErrorMessage = "Username or Email is required")]
    [Display(Name = "Username or Email")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; } = false;

    public string? ReturnUrl { get; set; }

    public List<Company> AvailableCompanies { get; set; } = new();
}

public class RegisterCompanyViewModel
{
    [Required(ErrorMessage = "Company Trade Name is required")]
    [Display(Name = "Company Trade Name")]
    public string CompanyName { get; set; } = string.Empty;

    [Display(Name = "Legal Registered Name")]
    public string? LegalName { get; set; }

    [Required(ErrorMessage = "Brand Short Code is required (e.g. ACME)")]
    [StringLength(6, MinimumLength = 2, ErrorMessage = "Brand Short Code must be 2 to 6 characters")]
    [Display(Name = "Brand Short Code / Acronym")]
    public string BrandShortName { get; set; } = string.Empty;

    [Display(Name = "GSTIN (Tax ID)")]
    public string? Gstin { get; set; }

    [Display(Name = "Base Currency")]
    public string BaseCurrency { get; set; } = "INR";

    [Display(Name = "Operating City")]
    public string? City { get; set; }

    // Primary Tenant Admin Details
    [Required(ErrorMessage = "Admin Full Name is required")]
    [Display(Name = "Administrator Full Name")]
    public string AdminFullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Admin Username is required")]
    [Display(Name = "Administrator Username")]
    public string AdminUsername { get; set; } = string.Empty;

    [Required(ErrorMessage = "Official Email is required")]
    [EmailAddress(ErrorMessage = "Please provide a valid email address")]
    [Display(Name = "Official Email Address")]
    public string AdminEmail { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    public string? AdminPhone { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string AdminPassword { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("AdminPassword", ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
