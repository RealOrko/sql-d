using Microsoft.AspNetCore.Mvc;
using SqlD.Extensions.Network.Server;
using SqlD.Network.Server.Api.Id.Model;

namespace SqlD.Network.Server.Api.Id.Controllers;

[ApiController]
[Route("api/id")]
public class IdController(EndPoint serverAddress) : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        return this.Intercept(() => Ok(new IdResponse(serverAddress, HttpContext.Request)));
    }
}