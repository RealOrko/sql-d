using SqlD.Configs.Model;
using SqlD.Network.Server;

namespace SqlD.Builders;

internal class NewListenerBuilder
{
    public ConnectionListener Hosting(SqlDServiceModel serviceModel)
    {
        var dbConnection = new DbConnection().Connect(serviceModel.Name, serviceModel.Database, serviceModel.Pragma);
        var connectionListener = ConnectionListenerFactory.Create(serviceModel, dbConnection);
        return connectionListener;
    }
}