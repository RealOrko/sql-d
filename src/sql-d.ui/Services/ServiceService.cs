using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlD.Configuration.Model;
using SqlD.Logging;
using SqlD.Network;
using SqlD.Network.Server.Api.Registry;
using SqlD.UI.Models.Registry;
using SqlD.UI.Models.Services;

namespace SqlD.UI.Services
{
	public class ServiceService
	{
		private readonly ConfigService config;
		private readonly RegistryService registry;

		public ServiceService() : this(new ConfigService(), new RegistryService())
		{
		}

		public ServiceService(ConfigService configService, RegistryService registryService)
		{
			this.config = configService;
			this.registry = registryService;
		}

		public async Task<RegistryViewModel> GetRegistry()
		{
			return await registry.GetServices();
		}

		public void AddServiceToConfigAndStart(ServiceFormViewModel service)
		{
			var config = this.config.Get();

			var sqlDServiceModel = new SqlDServiceModel()
			{
				Name = service.Name,
				Database = service.Database,
				Host = service.Host,
				Port = service.Port,
				Tags = (service.Tags ?? string.Empty).Split(',').ToList(),
			};

			var registryEntryViewModels = service.Forwards.Where(x => x.Selected).ToList();
			if (registryEntryViewModels.Any())
			{
				sqlDServiceModel.ForwardingTo.AddRange(registryEntryViewModels.Select(y => new SqlDForwardingModel()
				{
					Host = y.Host,
					Port = y.Port
				}));
			}

			config.Services.Add(sqlDServiceModel);

			this.config.Set(config);

			Interface.Start(typeof(ServiceService).Assembly, config);
		}

		public void UpdateServiceAndRestart(ServiceFormViewModel service)
		{
			var config = this.config.Get();

			var sqlDServiceModel = config.Services.First(x => x.ToEndPoint().Equals(new EndPoint(service.Host, service.Port)));

			var registryEntryViewModels = service.Forwards.Where(x => x.Selected).ToList();
			if (registryEntryViewModels.Any())
			{
				sqlDServiceModel.ForwardingTo = new List<SqlDForwardingModel>();
				sqlDServiceModel.ForwardingTo.AddRange(registryEntryViewModels.Select(y => new SqlDForwardingModel()
				{
					Host = y.Host,
					Port = y.Port
				}));
			}

			this.config.Set(config);

			KillService(sqlDServiceModel.Host, sqlDServiceModel.Port, removeFromConfig:false);

			Interface.Start(typeof(ServiceService).Assembly, config);
		}

		public void KillService(string host, int port, bool removeFromConfig)
		{
			var hostToKill = new EndPoint(host, port);

			try
			{
				Log.Out.Info($"Sending remote kill command to {hostToKill.ToUrl()}");
				Interface.NewClient(withRetries: false).ConnectedTo(hostToKill).Kill();
			}
			catch (Exception err)
			{
				Log.Out.Error(err.Message);
				Log.Out.Error(err.StackTrace);
			}

			try
			{
				Log.Out.Info($"Unregistering {hostToKill.ToUrl()}");
				Registry.Unregister(hostToKill);
			}
			catch (Exception err)
			{
				Log.Out.Error(err.Message);
				Log.Out.Error(err.StackTrace);
			}

			if (removeFromConfig)
			{
				try
				{
					Log.Out.Info($"Removing {hostToKill.ToUrl()} from config");
					var config = this.config.Get();
					config.Services = config.Services.Where(x => !x.ToEndPoint().Equals(hostToKill)).ToList();
					config.Services.ForEach(x => x.ForwardingTo = x.ForwardingTo.Where(x => !x.ToEndPoint().Equals(hostToKill)).ToList());
					this.config.Set(config);
				}
				catch (Exception err)
				{
					Log.Out.Error(err.Message);
					Log.Out.Error(err.StackTrace);
				}
			}
		}
	}
}