using System.Reflection;
using SqlD.Builders;
using SqlD.Configs;
using SqlD.Configs.Model;
using SqlD.Logging;
using SqlD.Network.Client;
using SqlD.Network.Diagnostics;
using SqlD.Network.Server;
using SqlD.Network.Server.Api.Registry;

namespace SqlD;

public static class Interface
{
    public static void Start(this Assembly startAssembly, SqlDConfiguration config = null)
    {
        var cfg = config ?? Configuration.Load(startAssembly);
        Start(cfg);
    }

    public static SqlDConfiguration Start(this Assembly startAssembly, string settingsFile)
    {
        var cfg = Configuration.Load(startAssembly, settingsFile);
        Start(cfg);
        return cfg;
    }

    private static void Start(SqlDConfiguration cfg)
    {
        if (cfg == null)
        {
            Log.Out.Warn("Configuration is null, are you sure this is what you intended?");
            return;
        }

        if (!cfg.Enabled) return;

        var registryEndPoints = cfg.Registries.ToList();
        var registries = cfg.Services.Where(x => registryEndPoints.Any(x.IsEqualTo));
        var services = cfg.Services.Where(x => !registryEndPoints.Any(x.IsEqualTo));

        foreach (var service in registries)
            try
            {
                var client = new NewClientBuilder(false).ConnectedTo(service);
                if (client.Ping())
                {
                    Log.Out.Warn($"Skipping the start of registry service '{service}', already up!");
                    continue;
                }

                var listener = new NewListenerBuilder().Hosting(service);
                EndPointMonitor.WaitUntil(service, EndPointIs.Up);
                Registry.Register(listener, string.Join(",", service.Tags));
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                throw;
            }

        foreach (var service in services)
            try
            {
                var client = new NewClientBuilder(false).ConnectedTo(service);
                if (client.Ping())
                {
                    Log.Out.Warn($"Skipping the start of sql service '{service}', already up!");
                    continue;
                }

                var listener = new NewListenerBuilder().Hosting(service);
                Registry.Register(listener, string.Join(",", service.Tags));
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                throw;
            }
    }

    public static void Stop()
    {
        Configuration.Reset();
        ConnectionClientFactory.DisposeAll();
        ConnectionListenerFactory.DisposeAll();
    }
}