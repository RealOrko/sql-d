using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models;
using SqlD.UI.Services;

namespace SqlD.UI.ViewComponents.Registry
{
	public class RegistryList : ViewComponent
	{
		private readonly RegistryService registry;

		public RegistryList()
		{
			this.registry = new RegistryService();
		}

		public async Task<IViewComponentResult> InvokeAsync(SqlLiteViewModel query)
		{
			var registryViewModel = await registry.GetServices();
			return View(registryViewModel);
		}
	}
}