using SqlD.Configs.Model;

namespace SqlD.UI.Services
{
    public class ConfigService
    {
        public SqlDConfiguration Get()
        {
            return Configs.Configuration.Load(typeof(ConfigService).Assembly);
        }

        public void Set(SqlDConfiguration config)
        {
            Configs.Configuration.Update(typeof(ConfigService).Assembly, config);
        }
    }
}