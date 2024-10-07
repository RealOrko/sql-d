using System.Linq;
using SqlD.Network;
using SqlD.Network.Client;
using SqlD.UI.Services.Query;

namespace SqlD.UI.Services.Client
{
	public class ClientFactory
	{
		public ConnectionClient GetClientOrDefault(QueryContext context, ConfigService config)
		{
			var sqlDConfiguration = config.Get();
			var cfg = sqlDConfiguration.Services.FirstOrDefault(x => x.Tags.Contains("master"));
			var client = Interface.NewClient().ConnectedTo(EndPoint.FromUri(context.TargetUri) ?? cfg.ToEndPoint());
			return client;
		}
	}
}