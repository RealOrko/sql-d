using SqlD.Configs.Model;

namespace SqlD.UI.Services
{
	public class SurfaceService
	{
		private readonly ConfigService config;

		public SurfaceService(ConfigService configService)
		{
			this.config = configService;
		}

		public SqlDConfiguration GetConfig()
		{
			return this.config.Get();
		}
	}
}