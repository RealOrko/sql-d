using System;
using System.Linq;
using System.Threading.Tasks;
using SqlD.Network.Client;
using SqlD.Network.Server.Api.Db.Model;
using SqlD.UI.Models.Query;

namespace SqlD.UI.Services.Query.Actions
{
	public class DescribeAction : IQueryAction
	{
		public async Task<object> Go(QueryContext context, ConnectionClient client, RegistryService registry)
		{
			var describe = context.Query.Split("?", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

			Describe describeRequest;
			if (describe.Any())
			{
				describeRequest = new Describe() { TableName = describe[0] };
			}
			else
			{
				describeRequest = new Describe() { TableName = "sqlite_master" };
			}

			try
			{
				var describeResponse = await client.DescribeCommandAsync(describeRequest);
				return new DescribeResultViewModel(describeResponse, await registry.GetServices(), context.HttpContext.Request);
			}
			catch (Exception err)
			{
				return new DescribeResultViewModel(err.Message);
			}
		}
	}
}