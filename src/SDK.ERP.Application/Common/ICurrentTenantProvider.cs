namespace SDK.ERP.Application.Common;

/// <summary>
/// Scoped tenant provider interface that supplies the active tenant/organization ID
/// to EF Core Global Query Filters and SaveChangesAsync write-interceptors.
/// </summary>
public interface ICurrentTenantProvider
{
    /// <summary>
    /// The currently active organization/tenant ID for the request.
    /// If 0, bypasses filtering (e.g. during system seed or initial tenant registration).
    /// </summary>
    long CurrentCompanyId { get; }

    /// <summary>
    /// Sets the active tenant for the current request/session.
    /// </summary>
    void SetTenant(long companyId);
}
