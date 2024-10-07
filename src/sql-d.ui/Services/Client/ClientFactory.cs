using System.Linq;
using SqlD.Builders;
using SqlD.Network;
using SqlD.Network.Client;
using SqlD.UI.Services.Query;

namespace SqlD.UI.Services.Client
{
	public class ClientFactory
	{
		public ConnectionClient GetClientOrDefault(string targetUri, ConfigService config)
		{
			var sqlDConfiguration = config.Get();
			var cfg = sqlDConfiguration.Services.FirstOrDefault(x => x.Tags.Contains("master"));
			var client = new NewClientBuilder(withRetries:true).ConnectedTo(EndPoint.FromUri(targetUri) ?? cfg.ToEndPoint());
			return client;
		}
	}
}