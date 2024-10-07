namespace SqlD.Configuration.Model
{
	public class SqlDConfiguration
    {
        public static SqlDConfiguration Default { get; } = new SqlDConfiguration()
        {
            Enabled = true,
            // ProcessModel = new SqlDProcessModel()
            // {
            //     Distributed = false
            // },
            Registries = new List<SqlDRegistryModel>()
            {
                new SqlDRegistryModel()
                {
                    Port = 5000,
                    Host = "localhost"
                }
            },
            Services = new List<SqlDServiceModel>()
            {
                new SqlDServiceModel()
                {
                    Database = "localhost.db",
                    Port = 5000,
                    Name = "localhost",
                    Host = "localhost",
                    Tags = new List<string>(){ "registry", "master", "localhost" }
                }
            }
        };

        public bool Enabled { get; set; } = true;
		public string Authority { get; set; }
		// public SqlDProcessModel ProcessModel { get; set; }
		public List<SqlDServiceModel> Services { get; set; } = new List<SqlDServiceModel>();
		public List<SqlDRegistryModel> Registries { get; set; } = new List<SqlDRegistryModel>();

		public override string ToString()
		{
			return $"{nameof(Enabled)}: {Enabled}, {nameof(Services)}: {Services}, {nameof(Registries)}: {Registries}";
		}
	}
}