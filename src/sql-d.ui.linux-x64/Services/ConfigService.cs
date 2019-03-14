using SqlD.Configuration;
using SqlD.Configuration.Model;

namespace SqlD.UI.Services
{
	public class ConfigService
	{
		public SqlDConfiguration Get()
		{
			return SqlDConfig.Get(typeof(ConfigService).Assembly);
		}

		public void Set(SqlDConfiguration config)
		{
			SqlDConfig.Set(typeof(ConfigService).Assembly, config);
		}
	}
}