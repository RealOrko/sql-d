using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models.Query;
using SqlD.UI.Services;

namespace SqlD.UI.ViewComponents.Sqlite
{
	public class SqliteEditorDescribeResults : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var query = base.HttpContext.Request.Query["q"].ToString();
			var server = base.HttpContext.Request.Query["s"].ToString();
			if (!string.IsNullOrEmpty(query))
			{
				try
				{
					var queryResponse = await new QueryService().Query(query, server, base.HttpContext);
					return View(queryResponse as DescribeResultViewModel);
				}
				catch (Exception err)
				{
					return View(new DescribeResultViewModel(err.Message));
				}
			}
			return View();
		}
	}
}