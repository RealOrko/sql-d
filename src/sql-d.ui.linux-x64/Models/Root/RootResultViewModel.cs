using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SqlD.UI.Models.Query;
using SqlD.UI.Models.Registry;

namespace SqlD.UI.Models.Root
{
	public class RootResultViewModel
	{
		public RootResultViewModel(QueryResultViewModel queryResultView, RegistryViewModel registry, HttpRequest httpContextRequest)
		{
			foreach (var result in queryResultView.Rows)
				if (result != null && result[0].ToString() == "table")
					Tables.Add(new RootTableResultViewModel(result[1].ToString(), registry, httpContextRequest));
		}

		public RootResultViewModel(string error)
		{
			Error = error;
		}

		public List<RootTableResultViewModel> Tables { get; } = new List<RootTableResultViewModel>();
		public string Error { get; set; }
	}
}