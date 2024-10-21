using System.Data.Common;
using System.Data.SQLite;

namespace SqlD;

public class DbReader : IDisposable
{
    public DbReader(SQLiteConnection connection)
    {
        Connection = connection;
        Command = connection.CreateCommand();
    }

    public SQLiteCommand Command { get; }
    public SQLiteConnection Connection { get; }
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