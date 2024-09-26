using System.Collections.Concurrent;
using SqlD.Configs.Model;
using SqlD.Logging;

namespace SqlD.Network.Server;

internal class ConnectionListenerFactory
{
    private static readonly ConcurrentDictionary<string, ConnectionListener> Listeners = new();

    internal static ConnectionListener Create(SqlDServiceModel serviceModel, DbConnectionFactory dbConnection)
    {
        return Listeners.GetOrAdd(serviceModel.ToUrl(), e =>
        {
            var connectionListener = new ConnectionListener();
            connectionListener.Listen(serviceModel, dbConnection);
            Events.RaiseListenerCreated(connectionListener);
            return connectionListener;
        });
    }

    internal static void Dispose(ConnectionListener listener)
    {
        Events.RaiseListenerDisposed(listener);
        Listeners.TryRemove(listener.ServiceModel.ToUrl(), out var l);
        listener.Dispose();
    }

    internal static void DisposeAll()
    {
        Log.Out.Info("Disposing all connection listeners.");
        Listeners.Values.ToList().ForEach(Dispose);
        Listeners.Clear();
    }

    public static void DisposeNotInConfig()
    {
        foreach (var listener in Listeners.Values.ToList())
        {
            if (!Configs.Configuration.Instance.Services.Any(service => service.IsEqualTo(listener.ServiceModel)))
            {
                Listeners.TryRemove(listener.ServiceModel.ToUrl(), out var _);
                listener.Dispose();
            }
        }
    }
}