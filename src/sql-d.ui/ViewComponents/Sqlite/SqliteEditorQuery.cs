using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models;

namespace SqlD.UI.ViewComponents.Sqlite;

public class SqliteEditorQuery : ViewComponent
{
    public IViewComponentResult Invoke(SqlLiteViewModel query)
    {
        return View();
    }
}