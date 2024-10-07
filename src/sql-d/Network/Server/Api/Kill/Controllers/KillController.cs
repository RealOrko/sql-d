using Microsoft.AspNetCore.Mvc;
using SqlD.Logging;
using SqlD.Network.Server.Api.Kill.Model;

namespace SqlD.Network.Server.Api.Kill.Controllers;

[ApiController]
[Route("api/kill")]
public class KillController : Controller
{
    private readonly EndPoint authorityAddress;

    public KillController(EndPoint serverAddress)
    {
        authorityAddress = serverAddress;
    }

    [HttpPost]
    public IActionResult Kill([FromBody] KillRequest request)
    {
        return this.Intercept(() =>
        {
            Log.Out.Warn($"Killing {request.EndPoint.ToUrl()}, sending unregister event ... ");
            Registry.Registry.Unregister(request.EndPoint);
            return Ok(new KillResponse(authorityAddress));
        });
    }
}