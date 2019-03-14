using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using SqlD.Network.Server.Api.Db.Model;
using SqlD.UI.Models.Discovery;
using SqlD.UI.Models.Registry;

namespace SqlD.UI.Models.Query
{
	public class QueryResultViewModel
	{
		public QueryResultViewModel(string error)
		{
			Error = error;
		}

		public QueryResultViewModel(QueryResponse response, RegistryViewModel registry, HttpRequest request)
		{
			Query = response.Query.Select;
			Columns = response.Query.Columns;
			Rows = response.Rows;

			Links.Add(new LinkViewModel($"/api", "tables", registry.Entries.First().GetRedirectedAuthorityUri(request), string.Empty, string.Empty));
			foreach (var registryEntry in registry.Entries)
			{
				if (registryEntry.Tags.Contains("registry")) continue;
				Links.Add(new LinkViewModel($"/api/query/{response.Query.Select}/{WebUtility.UrlEncode(registryEntry.GetRedirectedUri(request))}", "query", registryEntry.GetRedirectedAuthorityUri(request), registryEntry.GetRedirectedUri(request), registryEntry.Tags));
			}
		}

		public string Error { get; set; }
		public string Query { get; set; }
		public List<string> Columns { get; set; }
		public List<List<object>> Rows { get; set; }
		public List<LinkViewModel> Links { get; } = new List<LinkViewModel>();
	}
}