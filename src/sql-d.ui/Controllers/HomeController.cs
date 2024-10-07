using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models;

namespace SqlD.UI.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult IndexGet(string q = null, string s = null)
    {
        return View("Index", new SqlLiteViewModel { Query = q, Server = s });
    }

    [HttpPost("/")]
    public IActionResult IndexPost([FromForm] SqlLiteViewModel viewModel)
    {
        return PartialView("Index", viewModel);
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}