using System.Threading.Tasks;
using SqlD.Network.Client;
using SqlD.UI.Models.Query;

namespace SqlD.UI.Services.Query;

public class UnknownAction : IQueryAction
{
	public async Task<object> Go(string query, ConnectionClient client)
	{
		return await Task.FromResult(new UnknownResultViewModel(query));
	}
}