using System.Data.Common;
using System.Data.SQLite;

namespace SqlD
{
	public class DbReader : IDisposable
	{
		public SQLiteCommand Command { get; }
		public SQLiteConnection Connection { get; }
		public DbDataReader Reader { get; private set; }

		public DbReader(SQLiteConnection connection)
		{
			Connection = connection;
			Command = connection.CreateCommand();
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

		public void Dispose()
		{
			Command?.Dispose();
			Reader?.Dispose();
		}
	}
}