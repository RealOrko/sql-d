using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models;
using SqlD.UI.Models.Query;
using SqlD.UI.Services;

namespace SqlD.UI.ViewComponents.Sqlite
{
	public class SqliteEditorCommandResults : ViewComponent
	{
		private readonly QueryService queryService;

		public SqliteEditorCommandResults(QueryService queryService)
		{
			this.queryService = queryService;
		}

		public async Task<IViewComponentResult> InvokeAsync(SqlLiteViewModel query)
		{
			if (!string.IsNullOrEmpty(query.Query))
			{
				try
				{
					if (queryService.IsCommand(query.Query))
					{
						query.CommandResult = await queryService.Query(query.Query, query.Server) as CommandResultViewModel;
						return View(query);
					}
				}
				catch (Exception err)
				{
					query.CommandResult = new CommandResultViewModel(err.Message);
					return View(query);
				}
			}
			return View();
		}
	}
}