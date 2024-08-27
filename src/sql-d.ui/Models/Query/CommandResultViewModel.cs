using System.Collections.Generic;
using SqlD.Network.Server.Api.Db.Model;

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
		}

		public string Error { get; set; }
		public StatusCode Status { get; set; }
		public List<long> ScalarResults { get; set; }
	}
}