using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDK.ERP.Application.Common;
using SDK.ERP.Application.ViewModels;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace SDK.ERP.Web.Controllers;

public class ProfileController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ICompanyContext _companyContext;
    private readonly IWebHostEnvironment _env;

    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".svg", ".webp"
    };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public ProfileController(ApplicationDbContext db, ICompanyContext companyContext, IWebHostEnvironment env)
    {
        _db = db;
        _companyContext = companyContext;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Organization & Profile Settings";
        ViewData["ActiveMenu"] = "Admin";

        var currentCompany = await _companyContext.GetCurrentCompanyAsync();
        var currentUser = await _companyContext.GetCurrentUserAsync();
        var allCompanies = await _companyContext.GetAllCompaniesAsync();
        var branches = await _db.Branches.AsNoTracking().Where(b => b.CompanyId == currentCompany.Id).ToListAsync();
        var organizationUsers = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Branch)
            .Where(u => u.CompanyId == currentCompany.Id)
            .OrderBy(u => u.FullName)
            .AsNoTracking()
            .ToListAsync();
        var roles = await _db.Roles.OrderBy(r => r.RoleName).AsNoTracking().ToListAsync();

        var vm = new ProfileViewModel
        {
            Company = currentCompany,
            User = currentUser,
            AllCompanies = allCompanies,
            Branches = branches,
            OrganizationUsers = organizationUsers,
            Roles = roles,
            TotalProjectsCount = await _db.Projects.CountAsync(p => p.CompanyId == currentCompany.Id),
            TotalInvoicesCount = await _db.SalesInvoices.CountAsync(i => i.CompanyId == currentCompany.Id),
            TotalClientsCount = await _db.Clients.CountAsync(c => c.CompanyId == currentCompany.Id),
            TotalVendorsCount = await _db.Vendors.CountAsync(v => v.CompanyId == currentCompany.Id)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCompany(Company model, IFormFile? logoFile, bool removeLogo = false)
    {
        var company = await _db.Companies.FirstOrDefaultAsync(c => c.Id == model.Id);
        if (company == null)
        {
            company = new Company
            {
                CompanyCode = string.IsNullOrWhiteSpace(model.CompanyCode) ? "CORP-01" : model.CompanyCode.Trim().ToUpper(),
                CreatedAt = DateTime.UtcNow
            };
            _db.Companies.Add(company);
        }

        // Handle Corporate Logo Upload & Storage Partitioning
        if (removeLogo)
        {
            company.LogoUrl = null;
        }
        else if (logoFile != null && logoFile.Length > 0)
        {
            if (logoFile.Length > MaxFileSizeBytes)
            {
                TempData["ErrorMessage"] = "Logo file exceeds the maximum allowed size of 5 MB.";
                return RedirectToAction(nameof(Index));
            }

            var extension = Path.GetExtension(logoFile.FileName);
            if (string.IsNullOrEmpty(extension) || !AllowedImageExtensions.Contains(extension))
            {
                TempData["ErrorMessage"] = "Invalid image format. Allowed formats: PNG, JPG, JPEG, SVG, WebP.";
                return RedirectToAction(nameof(Index));
            }

            var safeDir = Path.Combine(_env.WebRootPath, "uploads", "tenants", $"org_{company.Id}", "branding");
            if (!Directory.Exists(safeDir)) Directory.CreateDirectory(safeDir);

            var safeFileName = $"logo_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
            var fullPath = Path.Combine(safeDir, safeFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await logoFile.CopyToAsync(stream);
            }

            company.LogoUrl = $"/uploads/tenants/org_{company.Id}/branding/{safeFileName}";
        }

        // Update Identity & Branding
        company.CompanyName = string.IsNullOrWhiteSpace(model.CompanyName) ? "My Enterprise ERP" : model.CompanyName.Trim();
        company.LegalName = string.IsNullOrWhiteSpace(model.LegalName) ? company.CompanyName : model.LegalName.Trim();
        company.BrandShortName = string.IsNullOrWhiteSpace(model.BrandShortName) 
            ? (company.CompanyName.Length <= 4 ? company.CompanyName.ToUpper() : company.CompanyName.Substring(0, 3).ToUpper())
            : model.BrandShortName.Trim().ToUpper();
        company.Tagline = string.IsNullOrWhiteSpace(model.Tagline) ? "ENTERPRISE ERP" : model.Tagline.Trim().ToUpper();
        company.Industry = model.Industry?.Trim();
        company.BaseCurrency = string.IsNullOrWhiteSpace(model.BaseCurrency) ? "INR" : model.BaseCurrency.Trim().ToUpper();

        // Update Tax & Legal
        company.Gstin = model.Gstin?.Trim().ToUpper();
        company.Pan = model.Pan?.Trim().ToUpper() ?? string.Empty;
        company.Tan = model.Tan?.Trim().ToUpper();
        company.Cin = model.Cin?.Trim().ToUpper();
        company.MsmeUdyamNo = model.MsmeUdyamNo?.Trim().ToUpper();

        // Update Contact & Address
        company.Email = model.Email?.Trim();
        company.Phone = model.Phone?.Trim();
        company.Website = model.Website?.Trim();
        company.AddressLine1 = model.AddressLine1?.Trim();
        company.AddressLine2 = model.AddressLine2?.Trim();
        company.City = model.City?.Trim();
        company.State = model.State?.Trim();
        company.StateCode = model.StateCode?.Trim();
        company.Pincode = model.Pincode?.Trim();

        // If GSTIN provided and state code is blank, extract state code from GSTIN
        if (!string.IsNullOrEmpty(company.Gstin) && company.Gstin.Length >= 2 && string.IsNullOrEmpty(company.StateCode))
        {
            company.StateCode = company.Gstin.Substring(0, 2);
        }

        // Update Invoicing & Banking Coordinates
        company.BankName = model.BankName?.Trim();
        company.BankAccountNumber = model.BankAccountNumber?.Trim();
        company.BankIfsc = model.BankIfsc?.Trim().ToUpper();
        company.BankBranch = model.BankBranch?.Trim();
        company.UpiId = model.UpiId?.Trim();
        company.AuthorizedSignatoryName = model.AuthorizedSignatoryName?.Trim();
        company.AuthorizedSignatoryDesignation = model.AuthorizedSignatoryDesignation?.Trim();
        company.TermsAndConditions = model.TermsAndConditions?.Trim();
        company.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _companyContext.ClearCache();

        TempData["SuccessMessage"] = $"<strong>{company.CompanyName}</strong> profile and brand logo updated successfully! All invoice templates, headers, and reports are now dynamically aligned.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUser(User model, IFormFile? avatarFile, bool removeAvatar = false)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
        if (user != null)
        {
            if (removeAvatar)
            {
                user.AvatarUrl = null;
            }
            else if (avatarFile != null && avatarFile.Length > 0)
            {
                if (avatarFile.Length > MaxFileSizeBytes)
                {
                    TempData["ErrorMessage"] = "Avatar photo exceeds maximum allowed size of 5 MB.";
                    return Redirect("/Profile#user");
                }

                var extension = Path.GetExtension(avatarFile.FileName);
                if (string.IsNullOrEmpty(extension) || !AllowedImageExtensions.Contains(extension))
                {
                    TempData["ErrorMessage"] = "Invalid image format. Allowed formats: PNG, JPG, JPEG, SVG, WebP.";
                    return Redirect("/Profile#user");
                }

                var safeDir = Path.Combine(_env.WebRootPath, "uploads", "tenants", $"org_{user.CompanyId}", "avatars");
                if (!Directory.Exists(safeDir)) Directory.CreateDirectory(safeDir);

                var safeFileName = $"avatar_user_{user.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension.ToLowerInvariant()}";
                var fullPath = Path.Combine(safeDir, safeFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                user.AvatarUrl = $"/uploads/tenants/org_{user.CompanyId}/avatars/{safeFileName}";
            }

            user.FullName = string.IsNullOrWhiteSpace(model.FullName) ? "Admin User" : model.FullName.Trim();
            user.Designation = string.IsNullOrWhiteSpace(model.Designation) ? "Administrator" : model.Designation.Trim();
            user.Email = string.IsNullOrWhiteSpace(model.Email) ? user.Email : model.Email.Trim();
            user.PhoneNumber = model.PhoneNumber?.Trim();

            await _db.SaveChangesAsync();
            _companyContext.ClearCache();

            TempData["SuccessMessage"] = $"User profile for <strong>{user.FullName}</strong> ({user.Designation}) updated successfully!";
        }

        return Redirect("/Profile#user");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(string fullName, string username, string email, string? designation, string? phoneNumber, int roleId, long? branchId, string? temporaryPassword)
    {
        var currentCompany = await _companyContext.GetCurrentCompanyAsync();
        
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
        {
            TempData["ErrorMessage"] = "Full Name, Username, and Email Address are required to add a team member.";
            return Redirect("/Profile#team");
        }

        // Check for duplicate username within the organization
        var exists = await _db.Users.AnyAsync(u => u.CompanyId == currentCompany.Id && u.Username == username.Trim());
        if (exists)
        {
            TempData["ErrorMessage"] = $"A user with username '{username}' already exists in your organization.";
            return Redirect("/Profile#team");
        }

        var defaultBranch = await _db.Branches.FirstOrDefaultAsync(b => b.CompanyId == currentCompany.Id);
        var selectedBranchId = branchId.HasValue && branchId.Value > 0 ? branchId.Value : (defaultBranch?.Id ?? 1);
        var selectedRoleId = roleId > 0 ? roleId : 1;

        var rawPassword = string.IsNullOrWhiteSpace(temporaryPassword) ? "Erp@2026!" : temporaryPassword.Trim();
        using var sha256 = SHA256.Create();
        var hashed = Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(rawPassword))).ToLowerInvariant();

        var newUser = new User
        {
            CompanyId = currentCompany.Id,
            BranchId = selectedBranchId,
            RoleId = selectedRoleId,
            Username = username.Trim().ToLowerInvariant(),
            FullName = fullName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Designation = string.IsNullOrWhiteSpace(designation) ? "Team Member" : designation.Trim(),
            PhoneNumber = phoneNumber?.Trim(),
            PasswordHash = hashed,
            IsActive = true
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"New team member <strong>{newUser.FullName}</strong> ({newUser.Designation}) added to {currentCompany.CompanyName} successfully! Default temporary password: <code>{rawPassword}</code>";
        return Redirect("/Profile#team");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUserStatus(long userId)
    {
        var currentCompany = await _companyContext.GetCurrentCompanyAsync();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == currentCompany.Id);

        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found or does not belong to your organization.";
            return Redirect("/Profile#team");
        }

        user.IsActive = !user.IsActive;
        await _db.SaveChangesAsync();

        var status = user.IsActive ? "activated" : "deactivated";
        TempData["SuccessMessage"] = $"User <strong>{user.FullName}</strong> has been {status} successfully.";
        return Redirect("/Profile#team");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCompany(Company model)
    {
        if (string.IsNullOrWhiteSpace(model.CompanyName))
        {
            TempData["ErrorMessage"] = "Company Name is required.";
            return RedirectToAction(nameof(Index));
        }

        var newCompany = new Company
        {
            CompanyCode = "CORP-" + (await _db.Companies.CountAsync() + 1).ToString("D2"),
            CompanyName = model.CompanyName.Trim(),
            LegalName = string.IsNullOrWhiteSpace(model.LegalName) ? model.CompanyName.Trim() : model.LegalName.Trim(),
            BrandShortName = string.IsNullOrWhiteSpace(model.BrandShortName) 
                ? (model.CompanyName.Length <= 4 ? model.CompanyName.ToUpper() : model.CompanyName.Substring(0, 3).ToUpper())
                : model.BrandShortName.Trim().ToUpper(),
            Tagline = string.IsNullOrWhiteSpace(model.Tagline) ? "ENTERPRISE ERP" : model.Tagline.Trim().ToUpper(),
            Industry = model.Industry?.Trim(),
            Gstin = model.Gstin?.Trim().ToUpper(),
            Pan = model.Pan?.Trim().ToUpper() ?? string.Empty,
            City = model.City?.Trim(),
            State = model.State?.Trim(),
            StateCode = model.StateCode?.Trim(),
            BaseCurrency = string.IsNullOrWhiteSpace(model.BaseCurrency) ? "INR" : model.BaseCurrency.Trim().ToUpper(),
            FinancialYearStart = new DateTime(DateTime.UtcNow.Year, 4, 1),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Companies.Add(newCompany);
        await _db.SaveChangesAsync();

        // Create Default Branch for the new Sister Company
        var branch = new Branch
        {
            CompanyId = newCompany.Id,
            BranchCode = $"{newCompany.BrandShortName}-HQ",
            BranchName = "Headquarters",
            AddressLine1 = newCompany.City ?? "Corporate Office",
            StateCode = newCompany.StateCode ?? "07",
            IsHeadOffice = true,
            IsActive = true
        };
        _db.Branches.Add(branch);
        await _db.SaveChangesAsync();

        // Associate Current User with the new Sister Company as Admin
        var currentUser = await _companyContext.GetCurrentUserAsync();
        var sisterUser = new User
        {
            CompanyId = newCompany.Id,
            BranchId = branch.Id,
            RoleId = currentUser.RoleId > 0 ? currentUser.RoleId : 1,
            Username = currentUser.Username,
            FullName = currentUser.FullName,
            Email = currentUser.Email,
            PhoneNumber = currentUser.PhoneNumber,
            PasswordHash = currentUser.PasswordHash,
            Designation = currentUser.Designation ?? "Managing Director / Owner",
            IsActive = true
        };
        _db.Users.Add(sisterUser);
        await _db.SaveChangesAsync();

        // Switch to the newly created company
        await _companyContext.SetCurrentCompanyAsync(newCompany.Id);
        _companyContext.ClearCache();

        TempData["SuccessMessage"] = $"New sister organization <strong>{newCompany.CompanyName}</strong> registered successfully and set as your active ERP workspace!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwitchCompany(long companyId)
    {
        var accessibleCompanies = await _companyContext.GetAllCompaniesAsync();
        if (!accessibleCompanies.Any(c => c.Id == companyId))
        {
            TempData["ErrorMessage"] = "Access Denied: You do not have authorization or an active user account in that organization.";
            return RedirectToAction(nameof(Index));
        }

        await _companyContext.SetCurrentCompanyAsync(companyId);
        _companyContext.ClearCache();
        var company = await _companyContext.GetCurrentCompanyAsync();
        TempData["SuccessMessage"] = $"Switched active organization to <strong>{company.CompanyName}</strong> ({company.BrandShortName}).";
        return RedirectToAction(nameof(Index));
    }
}
