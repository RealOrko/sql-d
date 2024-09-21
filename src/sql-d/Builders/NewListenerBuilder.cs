using SqlD.Configs.Model;
using SqlD.Network.Server;

namespace SqlD.Builders;

internal class NewListenerBuilder
{
    public ConnectionListener Hosting(SqlDServiceModel serviceModel)
    {
        var dbConnectionFactory = new DbConnectionFactory(serviceModel.Name, serviceModel.Database, serviceModel.Pragma);
        var connectionListener = ConnectionListenerFactory.Create(serviceModel, dbConnectionFactory);
        return connectionListener;
    }
}