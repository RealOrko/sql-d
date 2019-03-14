using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlD.UI.Models.Query;
using SqlD.UI.Models.Root;
using SqlD.UI.Services;

namespace SqlD.UI.Controllers.Api
{
    [ApiController]
	[Route("api")]
	public class RootController : Controller
	{
		private readonly QueryService queryService;
		private readonly RegistryService registryService;

		public RootController()
		{
			this.queryService = new QueryService();
			this.registryService = new RegistryService();
		}

		[HttpGet]
		public virtual async Task<IActionResult> Get()
		{
			try
			{
				var queryResultViewModel = await queryService.Query("select * from sqlite_master", null, HttpContext, cacheResult:false);
				var registryViewModel = await registryService.GetServices();
				var rootResultViewModel = new RootResultViewModel((QueryResultViewModel) queryResultViewModel, registryViewModel, HttpContext.Request);
				return Ok(rootResultViewModel);
			}
			catch (Exception err)
			{
				return StatusCode(500, new RootResultViewModel(err.Message));
			}
		}
	}
}