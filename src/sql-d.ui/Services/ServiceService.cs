using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlD.Builders;
using SqlD.Configs.Model;
using SqlD.Logging;
using SqlD.Network;
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
			var cfg = this.config.Get();

			var sqlDServiceModel = cfg.Services.First(x => x.ToEndPoint().Equals(new EndPoint(service.Host, service.Port)));

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

			this.config.Set(cfg);

			Interface.Stop();
			Interface.Start(typeof(ServiceService).Assembly, cfg);
		}

		public void KillService(string host, int port)
		{
			var hostToKill = new EndPoint(host, port);

			try
			{
				Log.Out.Info($"Sending remote kill command to {hostToKill.ToUrl()}");
				new NewClientBuilder(withRetries: false).ConnectedTo(hostToKill).Kill();
			}
			catch (Exception err)
			{
				Log.Out.Error(err.Message);
				Log.Out.Error(err.StackTrace);
			}

			try
			{
				Log.Out.Info($"Removing {hostToKill.ToUrl()} from config");
				var cfg = this.config.Get();
				cfg.Services = cfg.Services.Where(x => !x.ToEndPoint().Equals(hostToKill)).ToList();
				cfg.Services.ForEach(x => x.ForwardingTo = x.ForwardingTo.Where(x => !x.ToEndPoint().Equals(hostToKill)).ToList());
				this.config.Set(cfg);
			}
			catch (Exception err)
			{
				Log.Out.Error(err.Message);
				Log.Out.Error(err.StackTrace);
			}
			Interface.Stop();
			Interface.Start(typeof(ServiceService).Assembly, config.Get());
		}
	}
}