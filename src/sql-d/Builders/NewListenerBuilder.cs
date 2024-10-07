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
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (dbConnectionString == null) throw new ArgumentNullException(nameof(dbConnectionString));

			var dbConnection = new DbConnection().Connect(name, dbConnectionString, pragma);
			var connectionListener = ConnectionListenerFactory.Create(startAssembly, dbConnection, localEndPoint, forwardEndPoints);

			return connectionListener;
		}
	}
}