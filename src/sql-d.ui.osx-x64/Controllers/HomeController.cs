using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models;

namespace SqlD.UI.Controllers
{
	public class HomeController : Controller
    {
	    public IActionResult Index(string q = null, string s = null)
        {
            return View();
        }

		public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
