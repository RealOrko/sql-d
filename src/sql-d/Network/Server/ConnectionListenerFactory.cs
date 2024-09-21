using System.Collections.Concurrent;
using SqlD.Configs.Model;

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
        Listeners.Values.ToList().ForEach(Dispose);
        Listeners.Clear();
    }
}