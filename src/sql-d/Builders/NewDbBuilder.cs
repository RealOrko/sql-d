using SqlD.Configs.Model;

namespace SqlD.Builders;

internal class NewDbBuilder
{
    public DbConnection ConnectedTo(string databaseName, SqlDPragmaModel pragma)
    {
        return new DbConnection().Connect(databaseName, databaseName, pragma);
    }
}