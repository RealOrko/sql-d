using System.Data;
using System.Data.SQLite;
using SqlD.Configs.Model;
using SqlD.Exceptions;
using SqlD.Extensions;
using SqlD.Extensions.Discovery;
using SqlD.Logging;

namespace SqlD
{
	public class DbConnection : IDisposable
	{
		public string Name { get; private set; }
		public string DatabaseName { get; private set; }
		public string ConnectionString { get; private set; }
		public SQLiteConnection Connection { get; private set; }
		public SqlDPragmaModel PragmaOptions { get; private set; }

		public virtual DbConnection Connect(string name, string databaseName, SqlDPragmaModel pragmaOptions)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
			PragmaOptions = pragmaOptions ?? throw new ArgumentNullException(nameof(pragmaOptions));

			var builder = new SQLiteConnectionStringBuilder();
			builder.DataSource = databaseName;
			ConnectionString = $"{builder};cache=shared";
			Connection = CreateConnection();
			ApplyPragmaOptions(pragmaOptions);

			return this;
		}

		public List<T> Query<T>(string where = null) where T : new() => QueryExtensions.Query<T>(this, @where);

		public async Task<List<T>> QueryAsync<T>(string where = null) where T : new () => await QueryExtensions.QueryAsync<T>(this, @where);

		public virtual void Insert<T>(T instance)
		{
			using (var insertCommand = instance.GetInsertPrepared(this.Connection))
			{
				var result = insertCommand.ExecuteScalar();
				var idProperty = PropertyDiscovery.GetProperty("Id", typeof(T));
				idProperty.SetValue(instance, result);
			}
		}

		public virtual async Task InsertAsync<T>(T instance)
		{
			using (var insertCommand = instance.GetInsertPrepared(this.Connection))
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
			using (var updateCommand = instance.GetUpdatePrepared(this.Connection))
			{
				updateCommand.ExecuteNonQuery();
			}
		}

		public virtual async Task UpdateAsync<T>(T instance)
		{
			using (var updateCommand = instance.GetUpdatePrepared(this.Connection))
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
			if (!string.IsNullOrEmpty(pragmaOptions.QueryOnly))
			{
				this.ExecuteCommand($"PRAGMA QUERY_ONLY={pragmaOptions.QueryOnly};");
			}

			if (!string.IsNullOrEmpty(pragmaOptions.PageSize))
			{
				this.ExecuteCommand($"PRAGMA PAGE_SIZE={pragmaOptions.PageSize};");
			}

			if (!string.IsNullOrEmpty(pragmaOptions.CountChanges))
			{
				this.ExecuteCommand($"PRAGMA COUNT_CHANGES={pragmaOptions.CountChanges};");
			}

			if (!string.IsNullOrEmpty(pragmaOptions.JournalMode))
			{
				this.ExecuteCommand($"PRAGMA JOURNAL_MODE={pragmaOptions.JournalMode};");
			}

			if (!string.IsNullOrEmpty(pragmaOptions.LockingMode))
			{
				this.ExecuteCommand($"PRAGMA LOCKING_MODE={pragmaOptions.LockingMode};");
			}

			if (!string.IsNullOrEmpty(pragmaOptions.Synchronous))
			{
				this.ExecuteCommand($"PRAGMA SYNCHRONOUS={pragmaOptions.Synchronous};");
			}

			if (!string.IsNullOrEmpty(pragmaOptions.TempStore))
			{
				this.ExecuteCommand($"PRAGMA TEMP_STORE={pragmaOptions.TempStore};");
			}
		}

		internal SQLiteConnection CreateConnection()
		{
			if (!File.Exists(DatabaseName) && DatabaseName != ":memory:")
			{
				Log.Out.Info($"File does not exist, creating '{DatabaseName}' ... ");
				SQLiteConnection.CreateFile(DatabaseName);
			}

			var connection = new SQLiteConnection(ConnectionString);

			try
			{
				Log.Out.Info($"Connecting to '{DatabaseName}'");
				connection.Open();
			}
			catch (Exception err)
			{
				Log.Out.Error($"Failed connecting to '{DatabaseName}', {err}");
				throw new DbConnectionFailedException($"Failed connecting to '{DatabaseName}'", err);
			}

			return connection;
		}

		public void Dispose()
		{
			Connection?.Dispose();
		}
	}
}