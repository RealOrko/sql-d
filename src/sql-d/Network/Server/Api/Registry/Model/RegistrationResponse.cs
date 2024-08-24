namespace SqlD.Network.Server.Api.Registry.Model
{
	public class RegistrationResponse
	{
		public EndPoint Authority { get; set; }
		public Registration Registration { get; set; }
		public List<RegistryEntry> Registry { get; set; }
	}
}