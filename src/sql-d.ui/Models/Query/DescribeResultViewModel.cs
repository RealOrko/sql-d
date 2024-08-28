using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using SqlD.Network.Server.Api.Db.Model;
using SqlD.UI.Models.Registry;

namespace SqlD.UI.Models.Query
{
	public class DescribeResultViewModel
	{
		public DescribeResultViewModel(string error)
		{
			Error = error;
		}

		public DescribeResultViewModel(DescribeResponse response)
		{
			this.Table = response.Query.TableName;
			this.Columns = response.Columns;
			this.Rows = response.Results;
		}

		public string Error { get; set; }
		public string Table { get; set; }
		public string[] Columns { get; set; }
		public List<List<object>> Rows { get; set; }
	}
}