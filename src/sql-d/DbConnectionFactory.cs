using System.Collections.Concurrent;
using System.Data;
using System.Data.SQLite;
using SqlD.Configs.Model;
using SqlD.Exceptions;
using SqlD.Extensions;
using SqlD.Extensions.Discovery;
using SqlD.Logging;
using SqlD.Network.Client;

namespace SqlD;

public class DbConnectionFactory
{
    private static object _lock = new();
    private static readonly ConcurrentDictionary<string, DbConnection> Connections = new();
    
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
        var connectionsStrategy = Configs.Configuration.Instance.Settings.Connections.Strategy;
        
        if (connectionsStrategy == SqlDSettingsConnections.SINGLETON_STRATEGY)
        {
            if (Connections.TryGetValue(Name, out var connection))
            {
                return connection;
            }
        }
        
        lock (_lock)
        {
            if (connectionsStrategy == SqlDSettingsConnections.SINGLETON_STRATEGY)
            {
                return Connections.GetOrAdd(Name, (e) => new DbConnection.DbConnectionSingleton().Connect(Name, DatabaseName, PragmaOptions));
            }
            
            if (connectionsStrategy == SqlDSettingsConnections.FACTORY_STRATEGY)
            {
                Connections.GetOrAdd(Name, (e) => new DbConnection.DbConnectionSingleton().Connect(Name, DatabaseName, PragmaOptions)).DisposeSingleton();
                return new DbConnection().Connect(Name, DatabaseName, PragmaOptions);
            }
            
            throw new Exception($"Unknown db connection strategy '{connectionsStrategy}'.");
        }
    }
}