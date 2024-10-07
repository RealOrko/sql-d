using System.Threading.Tasks;
using SqlD.Network.Client;

namespace SqlD.UI.Services.Query.Actions
{
	public interface IQueryAction
	{
		Task<object> Go(string query, ConnectionClient client, RegistryService registry);
	}
}