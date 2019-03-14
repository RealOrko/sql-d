using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlD.Network;
using SqlD.UI.Models.Services;
using SqlD.UI.Services;

namespace SqlD.UI.Controllers
{
	public class ServiceController : Controller
	{
		private readonly ConfigService config;
		private readonly ServiceService services;

		public ServiceController()
		{
			this.config = new ConfigService();
			this.services = new ServiceService();
		}

		[HttpGet]
		public async Task<IActionResult> Index(string q = null, string s = null)
		{
			var registry = await services.GetRegistry();
			var viewModel = new ServiceViewModel(config.Get(), registry.Entries);
			return View(viewModel);
		}

		[HttpGet]
		public async Task<IActionResult> Edit([FromQuery] string host = null, [FromQuery] int port = 0)
		{
			var registry = await services.GetRegistry();
			var viewModel = new ServiceFormViewModel(registry.Entries);

			var registryEntry = registry.Entries.FirstOrDefault(x => x.EndPoint.Equals(new EndPoint(host, port)));
			if (registryEntry != null)
			{
				viewModel.Name = registryEntry.Name;
				viewModel.Host = registryEntry.Host;
				viewModel.Port = registryEntry.Port;
				viewModel.Database = registryEntry.Database;
				viewModel.Tags = string.Join(",", registryEntry.Tags);
			}

			var configEntry = config.Get().Services.FirstOrDefault(x => x.ToEndPoint().Equals(new EndPoint(host, port)));
			if (configEntry != null)
			{
				foreach (var forwardEntry in configEntry.ForwardingTo)
				{
					var forwardRegistryEntry = registry.Entries.First(x => x.EndPoint.Equals(new EndPoint(forwardEntry.Host, forwardEntry.Port)));
					forwardRegistryEntry.Selected = true;
					viewModel.Forwards.Add(forwardRegistryEntry);
				}
			}

			return View(viewModel);
		}

		[HttpPost]
		public IActionResult Edit([FromForm] ServiceFormViewModel formViewModel)
		{
			if (ModelState.IsValid)
			{
				services.UpdateServiceAndRestart(formViewModel);
				return Redirect("/service");
			}

			return View(formViewModel);
		}

		[HttpGet]
		public async Task<IActionResult> Launch()
		{
			var registry = await services.GetRegistry();
			return View(new ServiceFormViewModel(registry.Entries));
		}

		[HttpPost]
		public IActionResult Launch([FromForm] ServiceFormViewModel formViewModel)
		{
			if (ModelState.IsValid)
			{
				services.AddServiceToConfigAndStart(formViewModel);
				return Redirect("/service");
			}

			return View(formViewModel);
		}

		[HttpGet]
		public IActionResult Stop([FromQuery] string host, [FromQuery] int port)
		{
			services.KillService(host, port, removeFromConfig:true);
			return Redirect("/service");
		}
	}
}