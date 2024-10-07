using System.Reflection;
using SqlD.Builders;
using SqlD.Configs;
using SqlD.Configs.Model;
using SqlD.Logging;
using SqlD.Network;
using SqlD.Network.Client;
using SqlD.Network.Diagnostics;
using SqlD.Network.Server;
using SqlD.Network.Server.Api.Registry;

namespace SqlD;

public static class Interface
{
    public static void Setup(Assembly startAssembly, string settingsFile)
    {
        Configuration.SetAssembly(startAssembly);
        Configuration.SetSettingsFile(settingsFile);
    }
    
    public static void Start()
    {
        Start(Configuration.Instance);
    }

    private static void Start(SqlDConfiguration cfg)
    {
        if (!cfg.Enabled)
        {
            Log.Out.Warn("Configuration is disabled, exiting startup ...");
            return;
        }

        var registryEndPoints = cfg.Registries.ToList();
        var registries = cfg.Services.Where(x => registryEndPoints.Any(x.IsEqualTo));
        var services = cfg.Services.Where(x => !registryEndPoints.Any(x.IsEqualTo));

        foreach (var service in registries)
            try
            {
                var client = new NewClientBuilder(false).ConnectedTo(service);
                if (client.Ping())
                {
                    Log.Out.Warn($"Skipping the start of registry '{service}', already running.");
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
                    Log.Out.Warn($"Skipping the start of '{service}', already running.");
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

    public static void Stop(InterfaceStopKind stopKind = InterfaceStopKind.All, params EndPoint[] OptionalEndPoints)
    {
        Configuration.Reset();
        if (stopKind == InterfaceStopKind.All)
        {
            ConnectionClientFactory.DisposeAll();
            ConnectionListenerFactory.DisposeAll();
        }

        if (stopKind == InterfaceStopKind.NotFoundInConfig)
        {
            ConnectionClientFactory.DisposeNotInConfig(OptionalEndPoints);
            ConnectionListenerFactory.DisposeNotInConfig(OptionalEndPoints);
        }
    }

    public enum InterfaceStopKind
    {
        All,
        NotFoundInConfig
    }
}