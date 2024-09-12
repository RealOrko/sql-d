using Microsoft.AspNetCore.Mvc;

namespace SqlD.Network.Server.Api.Ping.Controllers;

[ApiController]
[Route("api/ping")]
public class PingController : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        return this.Intercept(Ok);
    }
}