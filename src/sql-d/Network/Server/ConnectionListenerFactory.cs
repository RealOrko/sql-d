using System.Collections.Concurrent;
using System.Reflection;

namespace SqlD.Network.Server
{
	internal class ConnectionListenerFactory
	{
		private static readonly ConcurrentDictionary<EndPoint, ConnectionListener> Listeners = new();

	    internal static ConnectionListener Find(EndPoint listenerEndPoint)
	    {
	        if (Listeners.TryGetValue(listenerEndPoint, out ConnectionListener listener))
	        {
	            return listener;
	        }
	        return null;
	    }

		internal static ConnectionListener Create(Assembly startAssembly, DbConnection dbConnection, EndPoint listenerEndPoint, EndPoint[] forwardEndPoints) => Listeners.GetOrAdd(listenerEndPoint, (e) =>
		{
			var connectionListener = new ConnectionListener();
			connectionListener.Listen(dbConnection, listenerEndPoint);
			Events.RaiseListenerCreated(connectionListener);
			return connectionListener;
		});

		internal static void Dispose(ConnectionListener listener)
		{
			Events.RaiseListenerDisposed(listener);
			Listeners.TryRemove(listener.EndPoint, out var l);
			listener.Dispose();
		}

		internal static void DisposeAll()
		{
			Listeners.Values.ToList().ForEach(Dispose);
		}
	}
}