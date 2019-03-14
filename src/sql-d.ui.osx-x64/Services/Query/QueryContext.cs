using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SqlD.UI.Services.Query
{
	public class QueryContext
	{
		private readonly string query;
		private readonly string targetUri;
		private readonly HttpContext httpContext;

		public QueryContext(string query, string targetUri = null, HttpContext httpContext = null)
		{
			this.targetUri = targetUri;
			this.httpContext = httpContext;
			this.query = FormatQuerySymbols(query);
		}

		public string Query => query;

		public string TargetUri => targetUri;

		public HttpContext HttpContext => httpContext;

		public async Task<object> If(QueryToken token, Func<Task<object>> action)
		{
			if (string.IsNullOrEmpty(query)) return false;
			if (query.ToLower().Trim().StartsWith(token.Value))
				return await action();
			return await Task.FromResult<object>(null);
		}

		public override string ToString()
		{
			return $"{nameof(query)}: {query}, {nameof(targetUri)}: {targetUri}";
		}

		private string FormatQuerySymbols(string q)
		{
			if (string.IsNullOrEmpty(q)) return q;

			if (q.ToLower().StartsWith("describe"))
			{
				q = q
					.Replace("DESCRIBE", "?")
					.Replace("Describe", "?")
					.Replace("describe", "?");
			}

			return q;
		}
	}
}