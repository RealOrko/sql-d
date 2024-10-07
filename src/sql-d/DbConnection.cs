using System.Data;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using SqlD.Configs.Model;
using SqlD.Exceptions;
using SqlD.Extensions;
using SqlD.Extensions.Discovery;
using SqlD.Logging;

namespace SqlD;

public class DbConnection : IDisposable
{
    public string Name { get; private set; }
    public string DatabaseName { get; private set; }
    public string ConnectionString { get; private set; }
    public SQLiteConnection Connection { get; private set; }
    public SqlDPragmaModel PragmaOptions { get; private set; }

    public void Dispose()
    {
        Connection?.Dispose();
    }

    internal virtual DbConnection Connect(string name, string databaseName, SqlDPragmaModel pragmaOptions)
    {
        Name = name;
        DatabaseName = databaseName;
        ConnectionString = CreateConnectionString(databaseName);
        Connection = CreateConnection(databaseName);
        ApplyPragmaOptions(pragmaOptions);
        return this;
    }
    
    internal string CreateConnectionString(string databaseName)
    {
        Log.Out.Info($"Start of creating connection string {databaseName} ... ");
        
        var builder = new SQLiteConnectionStringBuilder();
        if (databaseName == ":memory:")
        {
            builder.DataSource = databaseName;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(Configs.Configuration.Instance.DataDirectory))
            {
                Log.Out.Info($"Data directory does not exist for '{databaseName}' ... ");
                builder.DataSource = databaseName;
                
                if (!File.Exists(databaseName))
                {
                    Log.Out.Info($"File does not exist, creating '{databaseName}' ... ");
                    SQLiteConnection.CreateFile(databaseName);
                }
            }
            else
            {
                var databasePath = Path.Combine(Configs.Configuration.Instance.DataDirectory, databaseName);
                Log.Out.Info($"Data directory exists for '{databasePath}' ... ");
                builder.DataSource = databasePath;

                if (!File.Exists(databasePath))
                {
                    Log.Out.Info($"File does not exist, creating '{databasePath}' ... ");
                    SQLiteConnection.CreateFile(databasePath);
                }
            }
        }
        if (string.IsNullOrEmpty(builder.DataSource))
        {
            Log.Out.Info($"Data source does not exist for '{databaseName}' ... ");
            builder.DataSource = databaseName;
        }
        
        Log.Out.Info($"{builder};cache=shared");
        return $"{builder};cache=shared";
    }
    
    internal SQLiteConnection CreateConnection(string databaseName)
    {
        var connection = new SQLiteConnection(ConnectionString);

        try
        {
            Log.Out.Info($"Connecting to '{databaseName}'");
            connection.Open();
        }
        catch (Exception err)
        {
            Log.Out.Error($"Failed connecting to '{databaseName}', {err}");
            throw new DbConnectionFailedException($"Failed connecting to '{databaseName}'", err);
        }

        return connection;
    }

    public List<T> Query<T>(string where = null) where T : new()
    {
        return QueryExtensions.Query<T>(this, where);
    }

    public async Task<List<T>> QueryAsync<T>(string where = null) where T : new()
    {
        return await QueryExtensions.QueryAsync<T>(this, where);
    }

    public virtual void Insert<T>(T instance)
    {
        using (var insertCommand = instance.GetInsertPrepared(Connection))
        {
            var result = insertCommand.ExecuteScalar();
            var idProperty = PropertyDiscovery.GetProperty("Id", typeof(T));
            idProperty.SetValue(instance, result);
        }
    }

    public virtual async Task InsertAsync<T>(T instance)
    {
        using (var insertCommand = instance.GetInsertPrepared(Connection))
        {
            var result = await insertCommand.ExecuteScalarAsync();
            var idProperty = PropertyDiscovery.GetProperty("Id", typeof(T));
            idProperty.SetValue(instance, result);
        }
    }

    public virtual void InsertMany<T>(IEnumerable<T> instances)
    {
        using (var tx = Connection.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            foreach (var instance in instances)
                Insert(instance);
            tx.Commit();
        }
    }

    public virtual async Task InsertManyAsync<T>(IEnumerable<T> instances)
    {
        using (var tx = Connection.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            foreach (var instance in instances)
                await InsertAsync(instance);
            tx.Commit();
        }
    }

    public virtual void Update<T>(T instance)
    {
        using (var updateCommand = instance.GetUpdatePrepared(Connection))
        {
            updateCommand.ExecuteNonQuery();
        }
    }

    public virtual async Task UpdateAsync<T>(T instance)
    {
        using (var updateCommand = instance.GetUpdatePrepared(Connection))
        {
            await updateCommand.ExecuteNonQueryAsync();
        }
    }

    public virtual void UpdateMany<T>(IEnumerable<T> instances)
    {
        using (var tx = Connection.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            foreach (var instance in instances)
                Update(instance);
            tx.Commit();
        }
    }

    public virtual async Task UpdateManyAsync<T>(IEnumerable<T> instances)
    {
        using (var tx = Connection.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            foreach (var instance in instances)
                await UpdateAsync(instance);
            tx.Commit();
        }
    }

    public virtual void Delete<T>(T instance)
    {
        var insertCommand = instance.GetDelete();
        this.ExecuteCommand(insertCommand);
    }

    public virtual async Task DeleteAsync<T>(T instance)
    {
        var insertCommand = instance.GetDelete();
        await this.ExecuteCommandAsync(insertCommand);
    }

    public virtual void DeleteMany<T>(IEnumerable<T> instances)
    {
        using (var tx = Connection.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            foreach (var instance in instances)
                Delete(instance);
            tx.Commit();
        }
    }

    public virtual async Task DeleteManyAsync<T>(IEnumerable<T> instances)
    {
        using (var tx = Connection.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            foreach (var instance in instances)
                await DeleteAsync(instance);
            tx.Commit();
        }
    }

    public virtual void CreateTable<T>()
    {
        var createTable = typeof(T).GetCreateTable();
        this.ExecuteCommand(createTable);
    }

    public virtual async Task CreateTableAsync<T>()
    {
        var createTable = typeof(T).GetCreateTable();
        await this.ExecuteCommandAsync(createTable);
    }

    public virtual void DropTable<T>()
    {
        this.ExecuteCommand(typeof(T).GetDropTable());
    }

    public virtual async Task DropTableAsync<T>()
    {
        await this.ExecuteCommandAsync(typeof(T).GetDropTable());
    }

    public override string ToString()
    {
        return $"{nameof(DatabaseName)}: {DatabaseName}";
    }

    private void ApplyPragmaOptions(SqlDPragmaModel pragmaOptions)
    {
        if (!string.IsNullOrEmpty(pragmaOptions.QueryOnly)) this.ExecuteCommand($"PRAGMA QUERY_ONLY={pragmaOptions.QueryOnly};");
        if (!string.IsNullOrEmpty(pragmaOptions.PageSize)) this.ExecuteCommand($"PRAGMA PAGE_SIZE={pragmaOptions.PageSize};");
        if (!string.IsNullOrEmpty(pragmaOptions.CountChanges)) this.ExecuteCommand($"PRAGMA COUNT_CHANGES={pragmaOptions.CountChanges};");
        if (!string.IsNullOrEmpty(pragmaOptions.JournalMode)) this.ExecuteCommand($"PRAGMA JOURNAL_MODE={pragmaOptions.JournalMode};");
        if (!string.IsNullOrEmpty(pragmaOptions.LockingMode)) this.ExecuteCommand($"PRAGMA LOCKING_MODE={pragmaOptions.LockingMode};");
        if (!string.IsNullOrEmpty(pragmaOptions.Synchronous)) this.ExecuteCommand($"PRAGMA SYNCHRONOUS={pragmaOptions.Synchronous};");
        if (!string.IsNullOrEmpty(pragmaOptions.TempStore)) this.ExecuteCommand($"PRAGMA TEMP_STORE={pragmaOptions.TempStore};");
        PragmaOptions = pragmaOptions;
    }

    public FileStream GetDatabaseFileStream()
    {
        if (string.IsNullOrWhiteSpace(Configs.Configuration.Instance.DataDirectory))
        {
            if (File.Exists(DatabaseName))
            {
                Log.Out.Info($"The database file {DatabaseName} was found, opening new file stream.");
                return File.Open(DatabaseName, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }
        else
        {
            var databasePath = Path.Combine(Configs.Configuration.Instance.DataDirectory, DatabaseName);
            if (File.Exists(databasePath))
            {
                Log.Out.Info($"The database file {databasePath} was found, opening new file stream.");
                return File.Open(databasePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }
        throw new DbConnectionFailedException($"The database '{DatabaseName}' could not be opened for reading because it does not exist.", null);
    }
    
    public string GetDatabaseFileHash()
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(GetDatabaseFilePath()))
            {
                return BitConverter.ToString(md5.ComputeHash(stream));
            }
        }
    }
    
    public string GetDatabaseFilePath()
    {
        if (string.IsNullOrWhiteSpace(Configs.Configuration.Instance.DataDirectory))
        {
            if (File.Exists(DatabaseName))
            {
                Log.Out.Info($"The database file {DatabaseName} was found.");
                return DatabaseName;
            }
        }
        else
        {
            var databasePath = Path.Combine(Configs.Configuration.Instance.DataDirectory, DatabaseName);
            if (File.Exists(databasePath))
            {
                Log.Out.Info($"The database file {databasePath} was found.");
                return databasePath;
            }
        }
        throw new DbConnectionFailedException($"The database '{DatabaseName}' could not be opened for reading because it does not exist.", null);
    }
}