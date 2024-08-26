using Microsoft.AspNetCore.Mvc;
using SqlD.Configs.Model;
using SqlD.Network.Server.Api.Id.Model;

namespace SqlD.Network.Server.Api.Id.Controllers
{
    [ApiController]
    [Route("api/id")]
	public class IdController : Controller
	{
		private readonly EndPoint authorityAddress;

		public IdController(EndPoint serverAddress)
		{
			authorityAddress = serverAddress;
		}

		[HttpGet]
		public IActionResult Get()
		{
			return this.Intercept(() => Ok(new IdResponse(authorityAddress, HttpContext.Request)));
		}
	}
}