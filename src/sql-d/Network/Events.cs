using SqlD.Logging;
using SqlD.Network.Client;
using SqlD.Network.Server;

namespace SqlD.Network;

public static class Events
{
    public static event ClientCreatedEvent ClientCreated;
    public static event ClientDisposedEvent ClientDisposed;
    public static event ListenerCreatedEvent ListenerCreated;
    public static event ListenerDisposedEvent ListenerDisposed;

    internal static void RaiseClientCreated(ConnectionClient client)
    {
        Log.Out.Info($"Raising 'ClientCreated' event for {client.EndPoint}");
        ClientCreated?.Invoke(new ClientCreatedEventArgs(client));
    }

    public static void RaiseClientDisposed(ConnectionClient client)
    {
        Log.Out.Info($"Raising 'ClientDisposed' event for {client.EndPoint}");
        ClientDisposed?.Invoke(new ClientDisposedEventArgs(client));
    }

    internal static void RaiseListenerCreated(ConnectionListener listener)
    {
        Log.Out.Info($"Raising 'ListenerCreated' event for {listener.ServiceModel}");
        ListenerCreated?.Invoke(new ListenerCreatedEventArgs(listener));
    }

    public static void RaiseListenerDisposed(ConnectionListener listener)
    {
        Log.Out.Info($"Raising 'ListenerDisposed' event for {listener.ServiceModel}");
        ListenerDisposed?.Invoke(new ListenerDisposedEventArgs(listener));
    }
}

public delegate void ClientCreatedEvent(ClientCreatedEventArgs args);

public class ClientCreatedEventArgs
{
    public ClientCreatedEventArgs(ConnectionClient client)
    {
        Client = client;
    }

    public ConnectionClient Client { get; }

    public override string ToString()
    {
        return $"{nameof(Client)}: {Client.EndPoint}";
    }
}

public delegate void ClientDisposedEvent(ClientDisposedEventArgs args);

public class ClientDisposedEventArgs
{
    public ClientDisposedEventArgs(ConnectionClient client)
    {
        Client = client;
    }

    public ConnectionClient Client { get; }

    public override string ToString()
    {
        return $"{nameof(Client)}: {Client.EndPoint}";
    }
}

public delegate void ListenerCreatedEvent(ListenerCreatedEventArgs args);

public class ListenerCreatedEventArgs
{
    public ListenerCreatedEventArgs(ConnectionListener listener)
    {
        Listener = listener;
    }

    public ConnectionListener Listener { get; }

    public override string ToString()
    {
        return $"{nameof(Listener)}: {Listener.ServiceModel}";
    }
}

public delegate void ListenerDisposedEvent(ListenerDisposedEventArgs args);

public class ListenerDisposedEventArgs
{
    public ListenerDisposedEventArgs(ConnectionListener listener)
    {
        Listener = listener;
    }

    public ConnectionListener Listener { get; }

    public override string ToString()
    {
        return $"{nameof(Listener)}: {Listener.ServiceModel}";
    }
}