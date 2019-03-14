using Microsoft.AspNetCore.Http;

namespace SqlD.UI.Services.Query
{
	public class QueryCache
	{
		private const string QUERY_RESULT = "query-result";

		private readonly ContextService context;

		public QueryCache(HttpContext context):this(new ContextService(context))
		{ 
		}

		public QueryCache(ContextService context)
		{
			this.context = context;
		}

		public bool HasQueryResult()
		{
			return context.Has(QUERY_RESULT);
		}

		public object GetQueryResult()
		{
			return context.Get(QUERY_RESULT);
		}

		public object SetQueryResult(object result)
		{
			return context.Set(QUERY_RESULT, result);
		}
	}
}