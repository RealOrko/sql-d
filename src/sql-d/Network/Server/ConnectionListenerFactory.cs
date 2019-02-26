using System.Collections.Concurrent;
using System.Reflection;

namespace SqlD.Network.Server
{
	internal class ConnectionListenerFactory
	{
		private static readonly ConcurrentDictionary<EndPoint, ConnectionListener> Listeners = new ConcurrentDictionary<EndPoint, ConnectionListener>();

	    internal static ConnectionListener Find(EndPoint listenerEndPoint)
	    {
	        if (Listeners.TryGetValue(listenerEndPoint, out ConnectionListener listener))
	        {
	            return listener;
	        }
	        return null;
	    }

		internal static ConnectionListener Get(Assembly startAssembly, DbConnection dbConnection, EndPoint listenerEndPoint, EndPoint[] forwardEndPoints) => Listeners.GetOrAdd(listenerEndPoint, (e) =>
		{
			var connectionListener = new ConnectionListener();
			connectionListener.Listen(startAssembly, dbConnection, listenerEndPoint, forwardEndPoints);
			Events.RaiseListenerCreated(connectionListener);
			return connectionListener;
		});

		internal static void Remove(ConnectionListener listener)
		{
			Events.RaiseListenerDisposed(listener);
			Listeners.TryRemove(listener.EndPoint, out var c);
		}
	}
}