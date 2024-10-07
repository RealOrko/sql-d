using System.Collections.Concurrent;
using SqlD.Logging;

namespace SqlD.Network.Client;

internal class ConnectionClientFactory
{
    private static readonly ConcurrentDictionary<string, ConnectionClient> Clients = new();

    internal static ConnectionClient Create(EndPoint endPoint, bool withRetries, int retryLimit, int httpClientTimeoutMilliseconds)
    {
        return Clients.GetOrAdd(endPoint.ToUrl(), e =>
        {
            var connectionClient = new ConnectionClient(endPoint, withRetries, retryLimit, httpClientTimeoutMilliseconds);
            Events.RaiseClientCreated(connectionClient);
            return connectionClient;
        });
    }
    
    internal static void Dispose(ConnectionClient client)
    {
        Events.RaiseClientDisposed(client);
        Clients.TryRemove(client.EndPoint.ToUrl(), out var l);
        client.Dispose();
    }
    
    internal static void DisposeAll()
    {
        Log.Out.Info("Disposing all cached connection clients.");
        Clients.Values.ToList().ForEach(Dispose);
        Clients.Clear();
    }
}