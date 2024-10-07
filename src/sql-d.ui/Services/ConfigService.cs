using SqlD.Configs;
using SqlD.Configs.Model;

namespace SqlD.UI.Services;

public class ConfigService
{
    public SqlDConfiguration Get()
    {
        return Configuration.Load(typeof(ConfigService).Assembly);
    }

    public void Set(SqlDConfiguration config)
    {
        Configuration.Update(typeof(ConfigService).Assembly, config);
    }
}