using SqlD.Network;

namespace SqlD.Configuration.Model
{
	public class SqlDConfiguration
    {
        public static SqlDConfiguration Default { get; } = new SqlDConfiguration()
        {
            Enabled = true,
            Registries = new List<SqlDRegistryModel>()
            {
                new()
                {
                    Port = 5000,
                    Host = "localhost"
                }
            },
            Services = new List<SqlDServiceModel>()
            {
                new()
                {
                    Database = "localhost.db",
                    Port = 5000,
                    Name = "localhost",
                    Host = "localhost",
                    Tags = ["registry", "master", "localhost"]
                }
            }
        };

        public bool Enabled { get; set; } = true;
		public string Authority { get; set; }
		public List<SqlDServiceModel> Services { get; set; } = new();
		public List<SqlDRegistryModel> Registries { get; set; } = new();

        public List<EndPoint> FindForwardingAddresses(EndPoint listenerEndpoint)
        {
            var serviceListener = Services.First(x => x.ToEndPoint().Equals(listenerEndpoint));
            if (serviceListener.ForwardingTo != null)
            {
                return serviceListener.ForwardingTo.Select(x => x.ToEndPoint()).ToList();
            }
            return null;
        }

		public override string ToString()
		{
			return $"{nameof(Enabled)}: {Enabled}, {nameof(Services)}: {Services}, {nameof(Registries)}: {Registries}";
		}
	}
}