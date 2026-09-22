using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SDK.ERP.Application.Common;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Infrastructure.Data;

namespace SDK.ERP.Infrastructure.Services;

public class CompanyContext : ICompanyContext
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Company? _cachedCompany;
    private User? _cachedUser;

    public CompanyContext(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public long CurrentCompanyId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Session != null)
            {
                var sessionCompanyId = httpContext.Session.GetInt32("ActiveCompanyId");
                if (sessionCompanyId.HasValue && sessionCompanyId.Value > 0)
                {
                    return sessionCompanyId.Value;
                }
            }
            return 1;
        }
    }

    public void SetTenant(long companyId)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Session != null && companyId > 0)
        {
            httpContext.Session.SetInt32("ActiveCompanyId", (int)companyId);
        }
        ClearCache();
    }

    public async Task<Company> GetCurrentCompanyAsync()
    {
        if (_cachedCompany != null) return _cachedCompany;

        var httpContext = _httpContextAccessor.HttpContext;
        long? sessionCompanyId = null;

        if (httpContext?.Session != null)
        {
            sessionCompanyId = httpContext.Session.GetInt32("ActiveCompanyId");
        }

        Company? company = null;
        if (sessionCompanyId.HasValue && sessionCompanyId.Value > 0)
        {
            company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == sessionCompanyId.Value);
        }

        if (company == null)
        {
            company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync();
        }

        if (company == null)
        {
            company = new Company
            {
                CompanyCode = "CORP-01",
                CompanyName = "My Enterprise ERP",
                LegalName = "My Enterprise ERP",
                BrandShortName = "ERP",
                Tagline = "ENTERPRISE SUITE",
                BaseCurrency = "INR",
                City = "New Delhi",
                StateCode = "07",
                CreatedAt = DateTime.UtcNow
            };
        }

        // Ensure BrandShortName and Tagline have fallbacks if blank
        if (string.IsNullOrWhiteSpace(company.BrandShortName))
        {
            company.BrandShortName = GetInitials(company.CompanyName);
        }
        if (string.IsNullOrWhiteSpace(company.Tagline))
        {
            company.Tagline = "SOLUTIONS ERP";
        }

        _cachedCompany = company;
        return company;
    }

    public async Task<User> GetCurrentUserAsync()
    {
        if (_cachedUser != null) return _cachedUser;

        var user = await _db.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (user == null)
        {
            user = new User
            {
                Username = "admin",
                FullName = "Admin User",
                Designation = "Administrator",
                Email = "admin@erp.local",
                IsActive = true
            };
        }
        else
        {
            if (string.IsNullOrWhiteSpace(user.FullName)) user.FullName = "Admin User";
            if (string.IsNullOrWhiteSpace(user.Designation)) user.Designation = user.Role?.RoleName ?? "Administrator";
        }

        _cachedUser = user;
        return user;
    }

    public async Task SetCurrentCompanyAsync(long companyId)
    {
        var company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == companyId);
        if (company != null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Session != null)
            {
                httpContext.Session.SetInt32("ActiveCompanyId", (int)companyId);
            }
            _cachedCompany = company;
        }
    }

    public async Task<List<Company>> GetAllCompaniesAsync()
    {
        var currentUser = await GetCurrentUserAsync();

        // Platform host super admin (user 'admin' under seed company 1) can view/manage all organizations
        if (currentUser.Username == "admin" && currentUser.CompanyId == 1)
        {
            return await _db.Companies.AsNoTracking().OrderBy(c => c.CompanyName).ToListAsync();
        }

        // For regular enterprise users/owners, strictly return organizations where this user has an authorized account
        var emailLower = currentUser.Email?.Trim().ToLower() ?? string.Empty;
        var usernameLower = currentUser.Username.Trim().ToLower();

        var accessibleCompanyIds = await _db.Users
            .IgnoreQueryFilters()
            .Where(u => u.IsActive && 
                        (u.Id == currentUser.Id || 
                         (!string.IsNullOrEmpty(emailLower) && u.Email.ToLower() == emailLower) ||
                         u.Username.ToLower() == usernameLower))
            .Select(u => u.CompanyId)
            .Distinct()
            .ToListAsync();

        if (!accessibleCompanyIds.Contains(currentUser.CompanyId))
        {
            accessibleCompanyIds.Add(currentUser.CompanyId);
        }

        return await _db.Companies
            .IgnoreQueryFilters()
            .Where(c => accessibleCompanyIds.Contains(c.Id))
            .OrderBy(c => c.CompanyName)
            .ToListAsync();
    }

    public void ClearCache()
    {
        _cachedCompany = null;
        _cachedUser = null;
    }

    private static string GetInitials(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "ERP";
        var parts = text.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            return parts[0].Length <= 4 ? parts[0].ToUpper() : parts[0].Substring(0, 3).ToUpper();
        }
        var initials = string.Concat(parts.Take(3).Select(p => p[0])).ToUpper();
        return string.IsNullOrEmpty(initials) ? "ERP" : initials;
    }
}
