using SqlD.Configs.Model;

namespace SqlD.UI.Models.Surface
{
	public class SurfaceViewModel
	{
		public SqlDConfiguration Config { get; set; }

		public SurfaceViewModel(SqlDConfiguration config)
		{
			Config = config;
		}
	}
}