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

        var httpContext = _httpContextAccessor.HttpContext;
        User? user = null;

        // 1. Resolve from authenticated ClaimsPrincipal
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userIdStr = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (long.TryParse(userIdStr, out var claimUserId) && claimUserId > 0)
            {
                user = await _db.Users
                    .IgnoreQueryFilters()
                    .Include(u => u.Role)
                    .Include(u => u.Branch)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == claimUserId);
            }

            if (user == null)
            {
                var username = httpContext.User.Identity.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    user = await _db.Users
                        .IgnoreQueryFilters()
                        .Include(u => u.Role)
                        .Include(u => u.Branch)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
                }
            }
        }

        // 2. Resolve from Session ActiveUserId
        if (user == null && httpContext?.Session != null)
        {
            var sessionUserId = httpContext.Session.GetInt32("ActiveUserId");
            if (sessionUserId.HasValue && sessionUserId.Value > 0)
            {
                user = await _db.Users
                    .IgnoreQueryFilters()
                    .Include(u => u.Role)
                    .Include(u => u.Branch)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == sessionUserId.Value);
            }
        }

        // 3. Fallback to master admin or first active user
        if (user == null)
        {
            user = await _db.Users
                .IgnoreQueryFilters()
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .FirstOrDefaultAsync(u => u.Username == "admin" || (u.Role != null && u.Role.RoleName == "SUPER_ADMIN"))
                ?? await _db.Users
                    .IgnoreQueryFilters()
                    .Include(u => u.Role)
                    .Include(u => u.Branch)
                    .AsNoTracking()
                    .OrderBy(u => u.Id)
                    .FirstOrDefaultAsync();
        }

        if (user == null)
        {
            user = new User
            {
                Id = 1,
                Username = "admin",
                FullName = "Admin User",
                Designation = "Administrator",
                Email = "admin@sdksolutions.com",
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

        // Strictly check if current user is the master host platform owner:
        // Either username 'admin' OR root company (CompanyId 1) super admin.
        // Registered organization owners (like Kamal Adhikari) are NOT the master platform owner.
        var isMasterPlatformOwner = currentUser.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) 
                                 || (currentUser.CompanyId == 1 && (currentUser.Role?.RoleName == "SUPER_ADMIN" || currentUser.Id == 1));

        if (isMasterPlatformOwner)
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
