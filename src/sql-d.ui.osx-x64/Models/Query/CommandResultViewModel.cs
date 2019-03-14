using System.Collections.Generic;
using SqlD.Network.Server.Api.Db.Model;
using SqlD.UI.Models.Discovery;

namespace SqlD.UI.Models.Query
{
	public class CommandResultViewModel
	{
		public CommandResultViewModel(string error)
		{
			Error = error;
		}

		public CommandResultViewModel(CommandResponse response)
		{
			Status = response.StatusCode;
			ScalarResults = response.ScalarResults;
			Links.Add(new LinkViewModel($"/api", "tables", string.Empty, string.Empty, string.Empty));
		}

		public string Error { get; set; }
		public StatusCode Status { get; set; }
		public List<long> ScalarResults { get; set; }
		public List<LinkViewModel> Links { get; } = new List<LinkViewModel>();
	}
}