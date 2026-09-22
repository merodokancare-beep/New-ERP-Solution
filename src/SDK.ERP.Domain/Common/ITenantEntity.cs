namespace SDK.ERP.Domain.Common;

/// <summary>
/// Marker and contract interface for all domain entities that belong to an organization tenant.
/// Enforces compile-time and runtime tenant isolation to guarantee zero cross-tenant data leakage.
/// </summary>
public interface ITenantEntity
{
    long CompanyId { get; set; }
}
