using Microsoft.AspNetCore.Mvc;

namespace SqlD.UI.ViewComponents.Registry
{
	public class RegistrySelector : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}