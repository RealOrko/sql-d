using SqlD.Network;

namespace SqlD.Configs.Model
{
	public class SqlDConfiguration
    {
	    public static SqlDConfiguration Default()
	    {
		    return new()
		    {
			    Enabled = true,
			    Registries = new List<SqlDRegistryModel>(),
			    Services = new List<SqlDServiceModel>()
		    };
	    }

	    public bool Enabled { get; set; } = true;
		public List<SqlDServiceModel> Services { get; set; } = new();
		public List<SqlDRegistryModel> Registries { get; set; } = new();
		
		public IEnumerable<EndPoint> FindForwardingAddresses(EndPoint listenerEndpoint)
		{
			return Services.First(x => x.ToEndPoint() == listenerEndpoint).ForwardingTo.Select(x => x.ToEndPoint());
		}

		public override string ToString()
		{
			return $"{nameof(Enabled)}: {Enabled}, {nameof(Services)}: {Services}, {nameof(Registries)}: {Registries}";
		}
	}
}