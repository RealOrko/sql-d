using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlD.Extensions.Network.Server;
using SqlD.Network.Server.Api.Registry;
using SqlD.UI.Models.Registry;

namespace SqlD.UI.Services;

public class RegistryService
{
    public async Task<RegistryViewModel> GetServices()
    {
        var results = new List<RegistryEntryViewModel>();
        var registries = Configs.Configuration.Instance.Registries.ToList();
        foreach (var registry in registries)
        {
            var client = new RegistryClient(registry);
            var items = await client.ListAsync();
            var list = items.Convert(x => new RegistryEntryViewModel(x));
            results.AddRange(list);
        }
        return new RegistryViewModel(results);
    }
}