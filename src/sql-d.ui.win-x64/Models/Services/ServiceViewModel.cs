using System.Collections.Generic;
using SqlD.Configuration.Model;
using SqlD.UI.Models.Registry;

namespace SqlD.UI.Models.Services
{
	public class ServiceViewModel
	{
		public SqlDConfiguration Config { get; set; }
		public List<RegistryEntryViewModel> RegistryEntries { get; set; }

		public ServiceViewModel(SqlDConfiguration config, List<RegistryEntryViewModel> registryEntries)
		{
			Config = config;
			RegistryEntries = registryEntries;
		}

		public bool ContainsConfig(RegistryEntryViewModel service)
		{
			foreach (var configService in Config.Services)
			{
				if (service.EndPoint.Equals(configService.ToEndPoint()))
				{
					return true;
				}
			}

			return false;
		}
	}
}