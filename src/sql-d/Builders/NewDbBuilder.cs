using SqlD.Configs.Model;

namespace SqlD.Builders;

internal class NewDbBuilder
{
    public DbConnectionFactory ConnectedTo(string databaseName, SqlDPragmaModel pragma)
    {
        return new DbConnectionFactory(databaseName, databaseName, pragma);
    }
}