using SqlD.Configs.Model;

namespace SqlD.UI.Models.Surface;

public class SurfaceViewModel
{
    public SurfaceViewModel(SqlDConfiguration config)
    {
        Config = config;
    }

    public SqlDConfiguration Config { get; set; }
}