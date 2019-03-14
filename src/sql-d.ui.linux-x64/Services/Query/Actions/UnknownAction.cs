using System.Threading.Tasks;
using SqlD.Network.Client;
using SqlD.UI.Models.Query;

namespace SqlD.UI.Services.Query.Actions
{
	public class UnknownAction : IQueryAction
	{
		public async Task<object> Go(QueryContext context, ConnectionClient client, RegistryService registry)
		{
			return await Task.FromResult(new UnknownResultViewModel(context.Query));
		}
	}
}