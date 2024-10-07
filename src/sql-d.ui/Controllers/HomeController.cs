using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models;

namespace SqlD.UI.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index(string q = null, string s = null)
    {
        return View(new SqlLiteViewModel { Query = q, Server = s });
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}