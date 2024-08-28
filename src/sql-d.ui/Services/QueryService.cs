using System;
using System.Threading.Tasks;
using SqlD.UI.Services.Client;
using SqlD.UI.Services.Query.Actions;

namespace SqlD.UI.Services
{
	public class QueryService
	{
		private readonly ConfigService configService;
		private readonly RegistryService registryService;
		private readonly ClientFactory clientFactory;
		private readonly IQueryAction unknownAction;
		private readonly IQueryAction describeAction;
		private readonly IQueryAction commandAction;
		private readonly IQueryAction queryAction;

		public QueryService(ConfigService configService, RegistryService registryService, ClientFactory clientFactory, UnknownAction unknownAction, DescribeAction describeAction, CommandAction commandAction, QueryAction queryAction)
		{
			this.configService = configService ?? throw new ArgumentNullException(nameof(configService));
			this.registryService = registryService ?? throw new ArgumentNullException(nameof(registryService));
			this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
			this.unknownAction = unknownAction ?? throw new ArgumentNullException(nameof(unknownAction));
			this.describeAction = describeAction ?? throw new ArgumentNullException(nameof(describeAction));
			this.commandAction = commandAction ?? throw new ArgumentNullException(nameof(commandAction));
			this.queryAction = queryAction ?? throw new ArgumentNullException(nameof(queryAction));
		}

		public async Task<object> Query(string query, string targetUri = null)
		{
			var client = clientFactory.GetClientOrDefault(targetUri, configService);

			
			
			
			
			return await context.If(QueryToken.DESCRIBE, async () => await describeAction.Go(context, client, registryService))
			       ?? await context.If(QueryToken.SELECT, async () => await queryAction.Go(context, client, registryService))
			       ?? await context.If(QueryToken.INSERT, async () => await commandAction.Go(context, client, registryService))
			       ?? await context.If(QueryToken.UPDATE, async () => await commandAction.Go(context, client, registryService))
			       ?? await context.If(QueryToken.DELETE, async () => await commandAction.Go(context, client, registryService))
			       ?? await context.If(QueryToken.CREATE, async () => await commandAction.Go(context, client, registryService))
			       ?? await context.If(QueryToken.ALTER, async () => await commandAction.Go(context, client, registryService))
			       ?? await context.If(QueryToken.DROP, async () => await commandAction.Go(context, client, registryService))
			       ?? await unknownAction.Go(context, client, registryService);
		}

	}
}