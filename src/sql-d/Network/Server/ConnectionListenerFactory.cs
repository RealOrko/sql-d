using System.Collections.Concurrent;
using SqlD.Configs.Model;

namespace SqlD.Network.Server;

internal class ConnectionListenerFactory
{
    private static readonly ConcurrentDictionary<EndPoint, ConnectionListener> Listeners = new();

    internal static ConnectionListener Find(EndPoint listenerEndPoint)
    {
        if (Listeners.TryGetValue(listenerEndPoint, out var listener)) return listener;
        return null;
    }

    internal static ConnectionListener Create(SqlDServiceModel serviceModel, DbConnection dbConnection)
    {
        return Listeners.GetOrAdd(serviceModel, e =>
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
        Listeners.TryRemove(listener.ServiceModel, out var l);
        listener.Dispose();
    }

    internal static void DisposeAll()
    {
        Listeners.Values.ToList().ForEach(Dispose);
        Listeners.Clear();
    }
}