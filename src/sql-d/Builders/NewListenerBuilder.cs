using System.Reflection;
using SqlD.Configuration.Model;
using SqlD.Network;
using SqlD.Network.Server;

namespace SqlD.Builders
{
	public class NewListenerBuilder
	{
		internal NewListenerBuilder()
		{
		}

		public ConnectionListener Hosting(Assembly startAssembly, SqlDServiceModel serviceModel)
		{
			var dbConnection = new DbConnection().Connect(serviceModel.Name, serviceModel.Database, serviceModel.Pragma);
			var connectionListener = ConnectionListenerFactory.Create(startAssembly, dbConnection, serviceModel.ToEndPoint(), serviceModel.ForwardingTo.Select(x => x.ToEndPoint()).ToArray());
			return connectionListener;
		}

		public ConnectionListener Hosting(Assembly startAssembly, string name, string dbConnectionString, SqlDPragmaModel pragma, EndPoint localEndPoint, params EndPoint[] forwardEndPoints)
		{
			var serviceModel = new SqlDServiceModel()
			{
				Name = name,
				Database = dbConnectionString,
				Pragma = pragma,
				Host = localEndPoint.Host,
				Port = localEndPoint.Port,
				ForwardingTo = forwardEndPoints.Select(x => new SqlDForwardingModel()
				{
					Host = x.Host,
					Port = x.Port
				}).ToList()
			};

			Configs.Configuration.Instance.Services.Add(serviceModel);
			
			return Hosting(startAssembly, serviceModel);
		}
	}
}