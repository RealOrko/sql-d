﻿using System.Reflection;
using SqlD.Builders;
using SqlD.Configuration.Model;
using SqlD.Logging;
using SqlD.Network.Diagnostics;
using SqlD.Network.Server;
using SqlD.Network.Server.Api.Registry;

namespace SqlD
{
	public static class SqlDStart
	{
		public static string NewId()
		{
			return Guid.NewGuid().ToString("N");
		}

		public static NewDbBuilder NewDb()
		{
			return new NewDbBuilder();
		}

		public static NewClientBuilder NewClient(bool withRetries = true, int retryLimit = 5, int httpClientTimeoutMilliseconds = 5000)
		{
			return new NewClientBuilder(withRetries, retryLimit, httpClientTimeoutMilliseconds);
		}

		public static NewListenerBuilder NewListener()
		{
			return new NewListenerBuilder();
		}

		public static SqlDConfiguration SqlDGo(this Assembly startAssembly, SqlDConfiguration config = null)
		{
			var cfg = config ?? Configuration.SqlDConfig.Get(startAssembly);
			SqlD(cfg, startAssembly);
			return cfg;
		}

		public static SqlDConfiguration SqlDGo(this Assembly startAssembly, string settingsFile)
		{
			var cfg = Configuration.SqlDConfig.Get(startAssembly, settingsFile);
			SqlD(cfg, startAssembly);
			return cfg;
		}

		private static void SqlD(SqlDConfiguration cfg, Assembly startAssembly)
		{
			if (cfg == null)
			{
				Log.Out.Warn($"Configuration is null, are you sure this is what you intended?");
				return;
			}

			if (!cfg.Enabled) return;

			foreach (var registry in cfg.Registries)
			{
				Registry.GetOrAdd(registry.ToEndPoint());
			}

			var registryEndPoints = cfg.Registries.Select(x => x.ToEndPoint()).ToList();
			var serviceEndPointsThatAreRegistries = cfg.Services.Where(x => registryEndPoints.Contains(x.ToEndPoint())).ToList();
			var serviceEndPointsThatAreNotRegistries = cfg.Services.Where(x => !registryEndPoints.Contains(x.ToEndPoint())).ToList();

			foreach (var service in serviceEndPointsThatAreRegistries)
			{
				try
				{
					var client = NewClient(withRetries: false).ConnectedTo(service.ToEndPoint());
					if (client.Ping())
					{
						Log.Out.Warn($"Skipping the start of registry service '{service.ToEndPoint()}', already up!");
						continue;
					}

					var listener = NewListener().Hosting(startAssembly, service);
					EndPointMonitor.WaitUntil(service.ToEndPoint(), EndPointIs.Up);
					Registry.Register(listener, string.Join(",", service.Tags));
				}
				catch (Exception err)
				{
					Console.WriteLine(err.ToString());
					throw;
				}
			}

			foreach (var service in serviceEndPointsThatAreNotRegistries)
			{
				try
				{
					var client = NewClient(withRetries: false).ConnectedTo(service.ToEndPoint());
					if (client.Ping())
					{
						Log.Out.Warn($"Skipping the start of sql service '{service.ToEndPoint()}', already up!");
						continue;
					}

					var listener = NewListener().Hosting(startAssembly, service);
					Registry.Register(listener, string.Join(",", service.Tags));
				}
				catch (Exception err)
				{
					Console.WriteLine(err.ToString());
					throw;
				}
			}
		}

		public static void SqlDStop()
		{
			ConnectionListenerFactory.DisposeAll();
		}
	}
}