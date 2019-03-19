using SqlD.Configuration.Model;

namespace SqlD.UI.Services
{
	public class SurfaceService
	{
		private readonly ConfigService config;

		public SurfaceService()
		{
			this.config = new ConfigService();
		}

		public SqlDConfiguration GetConfig()
		{
			return this.config.Get();
		}
	}
}