using System.Linq;
using SqlD.Builders;
using SqlD.Network;
using SqlD.Network.Client;

namespace SqlD.UI.Services.Client;

public class ClientFactory
{
    public ConnectionClient GetClientOrDefault(string targetUri)
    {
        var cfg = Configs.Configuration.Instance.Services.FirstOrDefault(x => x.Tags.Contains("master"));
        var client = new NewClientBuilder(true).ConnectedTo(EndPoint.FromUri(targetUri) ?? cfg);
        return client;
    }
}