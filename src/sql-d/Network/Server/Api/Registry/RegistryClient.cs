using SqlD.Network.Client;
using SqlD.Network.Server.Api.Registry.Model;

namespace SqlD.Network.Server.Api.Registry
{
	public class RegistryClient
	{
		private const string REGISTRY_RESOURCE = "api/registry";

		private readonly ConnectionClient client;

		public RegistryClient(EndPoint endPoint)
		{
			this.client = SqlDStart.NewClient().ConnectedTo(endPoint);
		}

		public virtual List<RegistryEntry> List()
		{
			var response = client.Get<Registration, RegistrationResponse>(REGISTRY_RESOURCE);
			return response.Registry;
		}

		public virtual async Task<List<RegistryEntry>> ListAsync()
		{
			var response = await client.GetAsync<Registration, RegistrationResponse>(REGISTRY_RESOURCE);
			return response.Registry;
		}

		public virtual List<RegistryEntry> Register(string name, string database, EndPoint listenerEndPoint, params string[] tags)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (database == null) throw new ArgumentNullException(nameof(database));
			if (listenerEndPoint == null) throw new ArgumentNullException(nameof(listenerEndPoint));

			var response = client.Post<Registration, RegistrationResponse>(
				REGISTRY_RESOURCE,
				new Registration
				{
					Name = name, 
					Database = database,
					Source = listenerEndPoint,
					Tags = tags
				});

			return response.Registry;
		}

		public List<RegistryEntry> Unregister(EndPoint listenerEndPoint)
		{
			var response = client.Post<Registration, RegistrationResponse>(
				$"{REGISTRY_RESOURCE}/unregister",
				new Registration
				{
					Source = listenerEndPoint
				});

			return response.Registry;
		}

		public virtual void Push(EndPoint target)
		{
			var targetRegistry = SqlDStart.NewClient().ConnectedTo(target);
			var sourceRegistry = SqlDStart.NewClient().ConnectedTo(target);
			var registry = sourceRegistry.Get<Registration, RegistrationResponse>(REGISTRY_RESOURCE);
			foreach (var registryItem in registry.Registry)
			{
				if (!registryItem.Tags.Contains(Registry.REGISTRY))
				{
					targetRegistry.Post<Registration, RegistrationResponse>(
						REGISTRY_RESOURCE,
						new Registration
						{
							Name = registryItem.Name,
							Database = registryItem.Database,
							Source = registryItem.ToEndPoint(),
							Tags = registryItem.TagsAsArray
						});
				}
			}
		}
	}
}