using SDK.ERP.Domain.Entities.Accounting;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Application.ViewModels;

public class MastersViewModel
{
    public List<Client> Clients { get; set; } = new();
    public List<Vendor> Vendors { get; set; } = new();
    public List<Item> Items { get; set; } = new();
    public List<TaxRate> TaxRates { get; set; } = new();
    public List<AccountGroup> AccountGroups { get; set; } = new();
}
