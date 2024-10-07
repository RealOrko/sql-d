using System.Data;
using System.Data.SQLite;
using SqlD.Configs.Model;
using SqlD.Exceptions;
using SqlD.Extensions;
using SqlD.Extensions.Discovery;
using SqlD.Logging;

namespace SqlD;

public class DbConnectionFactory
{
    public string Name { get; }
    public string DatabaseName { get; }
    public SqlDPragmaModel PragmaOptions { get; }

    public DbConnectionFactory(string name, string databaseName, SqlDPragmaModel pragmaOptions)
    {
        Name = name;
        DatabaseName = databaseName;
        PragmaOptions = pragmaOptions;
    }

    public virtual DbConnection Connect()
    {
        return new DbConnection().Connect(Name, DatabaseName, PragmaOptions);
    }
}