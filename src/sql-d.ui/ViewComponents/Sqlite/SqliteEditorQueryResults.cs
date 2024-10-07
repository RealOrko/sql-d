﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models;
using SqlD.UI.Models.Query;
using SqlD.UI.Services;

namespace SqlD.UI.ViewComponents.Sqlite
{
	public class SqliteEditorQueryResults : ViewComponent
	{
		private readonly QueryService queryService;

		public SqliteEditorQueryResults(QueryService queryService)
		{
			this.queryService = queryService;
		}

		public async Task<IViewComponentResult> InvokeAsync(SqlLiteViewModel query = null)
		{
			if (!string.IsNullOrEmpty(query?.Query))
			{
				try
				{
					query.QueryResult = await queryService.Query(query.Query, query.Server) as QueryResultViewModel;
					return View(query);
				}
				catch (Exception err)
				{
					query.QueryResult = new QueryResultViewModel(err.Message);
					return View(query);
				}
			}
			return View();
		}
	}
}