using System.Collections.Immutable;
using SqlD.Logging;
using SqlD.Network.Diagnostics;
using SqlD.Network.Server.Api.Registry.Model;

namespace SqlD.Network.Server.Api.Registry
{
	public class Registry
	{
		public const string REGISTRY = "Registry";

		public static ImmutableArray<EndPoint> Get()
		{
			var endPoints = EndPointRegistry.Get();
			return endPoints.ToImmutableArray();
		}

		public static IEnumerable<RegistryEntry> List()
		{
			var availableRegistryEndPoints = Get();
			if (availableRegistryEndPoints.Any())
			{
				var results = new List<RegistryEntry>();
				foreach (var availableRegistryEndPoint in availableRegistryEndPoints)
				{
					var registryClient = new RegistryClient(availableRegistryEndPoint);
					results.AddRange(registryClient.List());
				}

				return results;
			}
			return Enumerable.Empty<RegistryEntry>();
		}

		public static void GetOrAdd(EndPoint registry)
		{
			EndPointRegistry.GetOrAdd(registry);
			Log.Out.Info($"Registry added {registry.ToUrl()}");
		}

		public static IEnumerable<RegistryEntry> Register(ConnectionListener listener, params string[] tags)
		{
			var availableRegistryEndPoints = Get();
			if (availableRegistryEndPoints.Any())
			{
				var results = new List<RegistryEntry>();
				foreach (var availableRegistryEndPoint in availableRegistryEndPoints)
				{
					var registryClient = new RegistryClient(availableRegistryEndPoint);
					registryClient.Register(listener.DbConnection.Name, listener.DbConnection.DatabaseName, listener.EndPoint, tags);
					Log.Out.Info($"Registered {listener.EndPoint.ToUrl()} -> {availableRegistryEndPoint.ToUrl()}");
					return List();
				}

				return results;
			}
			return Enumerable.Empty<RegistryEntry>();
		}

		public static IEnumerable<RegistryEntry> Unregister(EndPoint listenerEndPoint)
		{
			var availableRegistryEndPoints = Get();
			if (availableRegistryEndPoints.Any())
			{
				var results = new List<RegistryEntry>();
				foreach (var availableRegistryEndPoint in availableRegistryEndPoints)
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
					return List();
				}

				return results;
			}
			return Enumerable.Empty<RegistryEntry>();
		}
	}
}