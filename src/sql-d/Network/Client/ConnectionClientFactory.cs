using System.Collections.Concurrent;

namespace SqlD.Network.Client;

internal class ConnectionClientFactory
{
    private static readonly ConcurrentDictionary<EndPoint, ConnectionClient> Clients = new();

    internal static ConnectionClient Get(EndPoint endPoint, bool withRetries, int retryLimit, int httpClientTimeoutMilliseconds)
    {
        return Clients.GetOrAdd(endPoint, e =>
        {
            var connectionClient = new ConnectionClient(endPoint, withRetries, retryLimit, httpClientTimeoutMilliseconds);
            Events.RaiseClientCreated(connectionClient);
            return connectionClient;
        });
    }

    public static void Remove(ConnectionClient connectionClient)
    {
        Events.RaiseClientDisposed(connectionClient);
        Clients.TryRemove(connectionClient.EndPoint, out var c);
    }
}