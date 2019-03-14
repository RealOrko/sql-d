using Microsoft.AspNetCore.Mvc;

namespace SqlD.UI.Controllers
{
	public class RegistryController : Controller
	{
		public IActionResult List()
		{
			return ViewComponent("RegistryList");
		}

		public IActionResult Selector()
		{
			return ViewComponent("RegistrySelector");
		}
	}
}