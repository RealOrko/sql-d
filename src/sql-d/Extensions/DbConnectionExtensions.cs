using System.Data;

namespace SqlD.Extensions;

public static class DbConnectionExtensions
{
    public static void ExecuteCommand(this DbConnection connection, string commandSql)
    {
        using (var command = connection.Connection.CreateCommand())
        {
            command.CommandText = commandSql;
            command.ExecuteNonQuery();
        }
    }

    public static async Task ExecuteCommandAsync(this DbConnection connection, string commandSql)
    {
        using (var command = connection.Connection.CreateCommand())
        {
            command.CommandText = commandSql;
            await command.ExecuteNonQueryAsync();
        }
    }

    public static void ExecuteCommands(this DbConnection connection, IEnumerable<string> commandsSql)
    {
        using (var tx = connection.Connection.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            foreach (var commandSql in commandsSql)
                ExecuteCommand(connection, commandSql);
            tx.Commit();
        }
    }

    public static async Task ExecuteCommandsAsync(this DbConnection connection, IEnumerable<string> commandsSql)
    {
        using (var tx = connection.Connection.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            foreach (var commandSql in commandsSql)
                await ExecuteCommandAsync(connection, commandSql);
            tx.Commit();
        }
    }

    public static T ExecuteScalar<T>(this DbConnection connection, string commandSql)
    {
        using (var command = connection.Connection.CreateCommand())
        {
            command.CommandText = commandSql;
            var result = command.ExecuteScalar();
            return (T)result;
        }
    }

    public static async Task<T> ExecuteScalarAsync<T>(this DbConnection connection, string commandSql)
    {
        using (var command = connection.Connection.CreateCommand())
        {
            command.CommandText = commandSql;
            var result = await command.ExecuteScalarAsync();
            return (T)result;
        }
    }

    public static List<T> ExecuteScalars<T>(this DbConnection connection, IEnumerable<string> commandsSql)
    {
        var results = new List<T>();
        using (var tx = connection.Connection.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            foreach (var commandSql in commandsSql)
                results.Add(ExecuteScalar<T>(connection, commandSql));
            tx.Commit();
            return results;
        }
    }

    public static async Task<List<T>> ExecuteScalarsAsync<T>(this DbConnection connection, IEnumerable<string> commandsSql)
    {
        var results = new List<T>();
        using (var tx = connection.Connection.BeginTransaction())
        {
            foreach (var commandSql in commandsSql)
                results.Add(await ExecuteScalarAsync<T>(connection, commandSql));
            tx.Commit();
            return results;
        }
    }

    public static DbReader ExecuteQuery(this DbConnection connection, string querySql)
    {
        var dbReader = new DbReader(connection.Connection);
        dbReader.Command.CommandText = querySql;
        return dbReader.ExecuteReader();
    }

    public static async Task<DbReader> ExecuteQueryAsync(this DbConnection connection, string querySql)
    {
        var dbReader = new DbReader(connection.Connection);
        dbReader.Command.CommandText = querySql;
        return await dbReader.ExecuteReaderAsync();
    }
}