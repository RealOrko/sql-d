using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlD.Extensions.Network.Registry.Model;
using SqlD.Network.Server.Api.Registry;
using SqlD.UI.Models.Registry;

namespace SqlD.UI.Services
{
	public class RegistryService
	{
		private readonly ConfigService config;

		public RegistryService() : this(new ConfigService())
		{
		}

		public RegistryService(ConfigService configService)
		{
			this.config = configService;
		}

		public async Task<RegistryViewModel> GetServices()
		{
			var results = new List<RegistryEntryViewModel>();
			var registries = this.config.Get().Registries.ToList();
			foreach (var registry in registries)
			{
				var client = new RegistryClient(registry.ToEndPoint());
				var items = await client.ListAsync();
				var list = items.Convert(x => new RegistryEntryViewModel(x));
				results.AddRange(list);
			}

			return new RegistryViewModel(results);
		}
	}
}