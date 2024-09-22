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
    private static object _lock = new();
    private static DbConnection _connection;
    
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
        if (Configs.Configuration.Instance.Settings.Connections.Strategy == SqlDSettingsConnections.SINGLETON_STRATEGY && _connection != null)
        {
            return _connection;
        }
        
        lock (_lock)
        {
            if (Configs.Configuration.Instance.Settings.Connections.Strategy == SqlDSettingsConnections.SINGLETON_STRATEGY && _connection != null)
            {
                return _connection;
            }

            if (Configs.Configuration.Instance.Settings.Connections.Strategy == SqlDSettingsConnections.FACTORY_STRATEGY)
            {
                _connection?.DisposeSingleton();
                return new DbConnection().Connect(Name, DatabaseName, PragmaOptions);
            }
            
            if (Configs.Configuration.Instance.Settings.Connections.Strategy == SqlDSettingsConnections.SINGLETON_STRATEGY && _connection == null)
            {
                _connection = new DbConnection.DbConnectionSingleton().Connect(Name, DatabaseName, PragmaOptions);
            }
            
            return _connection;
        }
    }
}