using SqlD.Configs;
using SqlD.Logging;

namespace SqlD.Network.Server.Api.Registry;

public class Registry
{
    public static void Register(ConnectionListener listener, params string[] tags)
    {
        foreach (var availableRegistryEndPoint in Configuration.Instance.Registries)
        {
            var registryClient = new RegistryClient(availableRegistryEndPoint);
            registryClient.Register(listener.DbConnectionFactory.Name, listener.DbConnectionFactory.DatabaseName, listener.ServiceModel, tags);
            Log.Out.Info($"Registered {listener.ServiceModel.ToUrl()} -> {availableRegistryEndPoint.ToUrl()}");
        }
    }

    public static void Unregister(EndPoint listenerEndPoint)
    {
        foreach (var availableRegistryEndPoint in Configuration.Instance.Registries)
        {
            var registryClient = new RegistryClient(availableRegistryEndPoint);
            try
            {
                Log.Out.Info($"Trying to unregister {listenerEndPoint.ToUrl()} with registry {availableRegistryEndPoint.ToUrl()}");
                registryClient.Unregister(listenerEndPoint);
            }
            catch (Exception err)
            {
                Log.Out.Error($"{err.Message}");
                Log.Out.Error($"{err.StackTrace}");
            }

            Log.Out.Info($"Unregistered {listenerEndPoint.ToUrl()} -> {availableRegistryEndPoint.ToUrl()}");
        }
    }
}