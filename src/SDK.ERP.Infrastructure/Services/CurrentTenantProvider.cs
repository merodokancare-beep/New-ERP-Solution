using Microsoft.AspNetCore.Http;
using SDK.ERP.Application.Common;

namespace SDK.ERP.Infrastructure.Services;

/// <summary>
/// Scoped tenant resolver that extracts the active organization/tenant ID
/// from the current HTTP session or authenticated user identity.
/// Injected into ApplicationDbContext to drive Global Query Filters and SaveChanges validation.
/// </summary>
public class CurrentTenantProvider : ICurrentTenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private long? _overrideTenantId;

    public CurrentTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long CurrentCompanyId
    {
        get
        {
            if (_overrideTenantId.HasValue && _overrideTenantId.Value > 0)
            {
                return _overrideTenantId.Value;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Session != null)
            {
                var sessionCompanyId = httpContext.Session.GetInt32("ActiveCompanyId");
                if (sessionCompanyId.HasValue && sessionCompanyId.Value > 0)
                {
                    return sessionCompanyId.Value;
                }
            }

            // Fallback for initial requests / seed operations
            return 1;
        }
    }

    public void SetTenant(long companyId)
    {
        _overrideTenantId = companyId;
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Session != null && companyId > 0)
        {
            httpContext.Session.SetInt32("ActiveCompanyId", (int)companyId);
        }
    }
}
