using SqlD.Builders;
using SqlD.Network.Client;
using SqlD.Network.Server.Api.Registry.Model;

namespace SqlD.Network.Server.Api.Registry;

public class RegistryClient
{
    private const string REGISTRY_RESOURCE = "api/registry";

    private readonly ConnectionClient client;

    public RegistryClient(EndPoint endPoint)
    {
        client = new NewClientBuilder(true).ConnectedTo(endPoint);
    }

    public virtual async Task<List<RegistryEntry>> ListAsync()
    {
        var response = await client.GetAsync<Registration, RegistrationResponse>(REGISTRY_RESOURCE);
        return response.Registry;
    }

    public virtual void Register(string name, string database, EndPoint listenerEndPoint, params string[] tags)
    {
        client.Post<Registration, RegistrationResponse>(
            REGISTRY_RESOURCE,
            new Registration
            {
                Name = name,
                Database = database,
                Source = listenerEndPoint,
                Tags = tags
            });
    }

    public void Unregister(EndPoint listenerEndPoint)
    {
        client.Post<Registration, RegistrationResponse>(
            $"{REGISTRY_RESOURCE}/unregister",
            new Registration
            {
                Source = listenerEndPoint
            });
    }
}