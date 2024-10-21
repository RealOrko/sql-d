using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlD.Network;
using SqlD.UI.Models;
using SqlD.UI.Models.Query;
using SqlD.UI.Services;

namespace SqlD.UI.ViewComponents.Sqlite;

public class SqliteEditorQueryResults : ViewComponent
{
    private readonly QueryService queryService;

    public SqliteEditorQueryResults(QueryService queryService)
    {
        this.queryService = queryService;
    }

    public async Task<IViewComponentResult> InvokeAsync(SqlLiteViewModel query = null)
    {
        try
        {
            if (queryService.IsQuery(query.Query))
            {
                query.QueryResult = await queryService.Query(query.Query, query.Server) as QueryResultViewModel;
                return View(query);
            }
        }
        catch (Exception err)
        {
            query.QueryResult = new QueryResultViewModel(EndPoint.FromUri(query.Server), err.Message);
            return View(query);
        }

        return View();
    }
}