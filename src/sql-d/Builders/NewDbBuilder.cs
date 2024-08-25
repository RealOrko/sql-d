using SqlD.Configs.Model;

namespace SqlD.Builders
{
	public class NewDbBuilder
	{
		internal NewDbBuilder()
		{
		}

		public DbConnection ConnectedTo(string databaseName, SqlDPragmaModel pragma)
		{
			return new DbConnection().Connect(databaseName, databaseName, pragma);
		}

		public DbConnection ConnectedTo(string name, string databaseName, SqlDPragmaModel pragma)
		{
			return new DbConnection().Connect(name, databaseName, pragma);
		}
	}
}