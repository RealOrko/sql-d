using System;
using System.Threading.Tasks;
using SqlD.UI.Services.Client;
using SqlD.UI.Services.Query;

namespace SqlD.UI.Services
{
	public class QueryService
	{
		private readonly ConfigService configService;
		private readonly ClientFactory clientFactory;
		private readonly IQueryAction unknownAction;
		private readonly IQueryAction describeAction;
		private readonly IQueryAction commandAction;
		private readonly IQueryAction queryAction;

		public QueryService(ConfigService configService, ClientFactory clientFactory, UnknownAction unknownAction, DescribeAction describeAction, CommandAction commandAction, QueryAction queryAction)
		{
			this.configService = configService ?? throw new ArgumentNullException(nameof(configService));
			this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
			this.unknownAction = unknownAction ?? throw new ArgumentNullException(nameof(unknownAction));
			this.describeAction = describeAction ?? throw new ArgumentNullException(nameof(describeAction));
			this.commandAction = commandAction ?? throw new ArgumentNullException(nameof(commandAction));
			this.queryAction = queryAction ?? throw new ArgumentNullException(nameof(queryAction));
		}

		public async Task<object> Query(string query, string targetUri = null)
		{
			var client = clientFactory.GetClientOrDefault(targetUri, configService);
			return await If(query, QueryToken.DESCRIBE, async () => await describeAction.Go(query, client))
			       ?? await If(query, QueryToken.SELECT, async () => await queryAction.Go(query, client))
			       ?? await If(query, QueryToken.INSERT, async () => await commandAction.Go(query, client))
			       ?? await If(query, QueryToken.UPDATE, async () => await commandAction.Go(query, client))
			       ?? await If(query, QueryToken.DELETE, async () => await commandAction.Go(query, client))
			       ?? await If(query, QueryToken.CREATE, async () => await commandAction.Go(query, client))
			       ?? await If(query, QueryToken.ALTER, async () => await commandAction.Go(query, client))
			       ?? await If(query, QueryToken.DROP, async () => await commandAction.Go(query, client))
			       ?? await unknownAction.Go(query, client);
		}

		public bool IsCommand(string query)
		{
			if (string.IsNullOrEmpty(query)) return false;
			if (query.ToLower().Trim().StartsWith(QueryToken.INSERT.Value))
				return true;
			if (query.ToLower().Trim().StartsWith(QueryToken.UPDATE.Value))
				return true;
			if (query.ToLower().Trim().StartsWith(QueryToken.DELETE.Value))
				return true;
			if (query.ToLower().Trim().StartsWith(QueryToken.CREATE.Value))
				return true;
			if (query.ToLower().Trim().StartsWith(QueryToken.ALTER.Value))
				return true;
			if (query.ToLower().Trim().StartsWith(QueryToken.DROP.Value))
				return true;
			return false;
		}

		public bool IsDescribe(string query)
		{
			if (string.IsNullOrEmpty(query)) return false;
			if (query.ToLower().Trim().StartsWith(QueryToken.DESCRIBE.Value))
				return true;
			return false;
		}

		public bool IsQuery(string query)
		{
			if (string.IsNullOrEmpty(query)) return false;
			if (query.ToLower().Trim().StartsWith(QueryToken.SELECT.Value))
				return true;
			return false;
		}
		
		public async Task<object> If(string query, QueryToken token, Func<Task<object>> action)
		{
			if (string.IsNullOrEmpty(query)) return false;
			if (query.ToLower().Trim().StartsWith(token.Value))
				return await action();
			return await Task.FromResult<object>(null);
		}
	}
}