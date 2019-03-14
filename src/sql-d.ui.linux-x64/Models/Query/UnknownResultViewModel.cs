using System.Collections.Generic;

namespace SqlD.UI.Models.Query
{
	public class UnknownResultViewModel
	{
		public string Message { get; }
		public List<string> Examples { get; } = new List<string>()
		{
			"api/query/describe sqlite_master",
			"api/query/select * from sqlite_master"
		};

		public UnknownResultViewModel(string query)
		{
			Message = $"Unable to parse '{query}' into anything meaningful, please try again";
		}
	}
}