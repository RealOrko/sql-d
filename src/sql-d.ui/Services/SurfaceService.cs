using SqlD.Configs.Model;

namespace SqlD.UI.Services;

public class SurfaceService
{
    public SqlDConfiguration GetConfig()
    {
        return Configs.Configuration.Instance;
    }
}