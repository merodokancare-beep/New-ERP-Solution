using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Application.ViewModels;

public class ProfileViewModel
{
    public Company Company { get; set; } = new Company();
    public User User { get; set; } = new User();
    public List<Company> AllCompanies { get; set; } = new List<Company>();
    public List<Branch> Branches { get; set; } = new List<Branch>();
    public List<User> OrganizationUsers { get; set; } = new List<User>();
    public List<Role> Roles { get; set; } = new List<Role>();
    public int TotalProjectsCount { get; set; }
    public int TotalInvoicesCount { get; set; }
    public int TotalClientsCount { get; set; }
    public int TotalVendorsCount { get; set; }
}
