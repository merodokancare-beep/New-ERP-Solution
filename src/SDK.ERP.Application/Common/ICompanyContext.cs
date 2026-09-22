using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Application.Common;

public interface ICompanyContext : ICurrentTenantProvider
{
    Task<Company> GetCurrentCompanyAsync();
    Task<User> GetCurrentUserAsync();
    Task SetCurrentCompanyAsync(long companyId);
    Task<List<Company>> GetAllCompaniesAsync();
    void ClearCache();
}
