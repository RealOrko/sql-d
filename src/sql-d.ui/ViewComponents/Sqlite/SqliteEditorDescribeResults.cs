using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models;
using SqlD.UI.Models.Query;
using SqlD.UI.Services;

namespace SqlD.UI.ViewComponents.Sqlite
{
	public class SqliteEditorDescribeResults : ViewComponent
	{
		private readonly QueryService queryService;

		public SqliteEditorDescribeResults(QueryService queryService)
		{
			this.queryService = queryService;
		}

		public async Task<IViewComponentResult> InvokeAsync(SqlLiteViewModel query)
		{
			if (!string.IsNullOrEmpty(query.Query))
			{
				try
				{
					if (queryService.IsDescribe(query.Query))
					{
						query.DescribeResult = await queryService.Query(query.Query, query.Server) as DescribeResultViewModel;
						return View(query);
					}
				}
				catch (Exception err)
				{
					query.DescribeResult = new DescribeResultViewModel(err.Message);
					return View(query);
				}
			}
			return View();
		}
	}
}