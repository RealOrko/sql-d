using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace SqlD;

public class DbReader : IDisposable
{
    public DbReader(SqliteConnection connection)
    {
        Connection = connection;
        Command = connection.CreateCommand();
    }

    public SqliteCommand Command { get; }
    public SqliteConnection Connection { get; }
    public DbDataReader Reader { get; private set; }

    public void Dispose()
    {
        Command?.Dispose();
        Reader?.Dispose();
    }

    public DbReader ExecuteReader()
    {
        Reader = Command.ExecuteReader();
        return this;
    }

    public async Task<DbReader> ExecuteReaderAsync()
    {
        Reader = await Command.ExecuteReaderAsync();
        return this;
    }
}