using System.Threading.Tasks;
using SqlD.Network.Client;

namespace SqlD.UI.Services.Query;

public interface IQueryAction
{
	Task<object> Go(string query, ConnectionClient client);
}