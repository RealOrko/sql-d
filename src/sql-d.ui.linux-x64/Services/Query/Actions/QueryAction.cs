using System;
using System.Linq;
using System.Threading.Tasks;
using SqlD.Network.Client;
using SqlD.Network.Server.Api.Db.Model;
using SqlD.UI.Models.Query;

namespace SqlD.UI.Services.Query.Actions
{
	public class QueryAction : IQueryAction
	{
		public async Task<object> Go(QueryContext context, ConnectionClient client, RegistryService registry)
		{
			var from = context.Query.Split("from", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().TrimEnd(';')).ToList();
			var fromTableName = @from[1].Split(' ')[0];

			var select = @from[0].Split("select", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().TrimEnd(';')).ToList();
			var columnsOrWildcard = @select[0].Trim();

			string[] columns;

			if (columnsOrWildcard == "*")
			{
				if (fromTableName.ToLower().Equals("sqlite_master"))
				{
					columns = Describe.DescribeMasterColumns.ToArray();
				}
				else
				{
					columns = (await client.DescribeCommandAsync(new Describe() { TableName = fromTableName })).Results.Select(DescribeResponse.GetName).ToArray();
				}
			}
			else
			{
				columns = @select[0].Split(',', StringSplitOptions.RemoveEmptyEntries);
			}

			var q = new Network.Server.Api.Db.Model.Query()
			{
				Select = context.Query,
				Columns = columns.Select(x => x?.Trim()).ToList(),
				Properties = columns.Select(x => x?.Trim()).ToList()
			};

			try
			{
				var res = await client.PostQueryAsync(q);
				return new QueryResultViewModel(res, await registry.GetServices(), context.HttpContext.Request);
			}
			catch (Exception err)
			{
				return new QueryResultViewModel(err.Message);
			}
		}
	}
}