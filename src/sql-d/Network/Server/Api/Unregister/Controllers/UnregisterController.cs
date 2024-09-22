using Microsoft.AspNetCore.Mvc;
using SqlD.Extensions.Network.Server;
using SqlD.Logging;
using SqlD.Network.Server.Api.Unregister.Model;

namespace SqlD.Network.Server.Api.Unregister.Controllers;

[ApiController]
[Route("api/unregister")]
public class UnregisterController : Controller
{
    private readonly EndPoint _authorityAddress;

    public UnregisterController(EndPoint serverAddress)
    {
        _authorityAddress = serverAddress;
    }

    [HttpPost]
    public IActionResult Unregister([FromBody] UnregisterRequest request)
    {
        return this.Intercept(() =>
        {
            Log.Out.Warn($"Unregistering {request.EndPoint.ToUrl()}, sending unregister event ... ");
            Registry.Registry.Unregister(request.EndPoint);
            return Ok(new UnregisterResponse(_authorityAddress));
        });
    }
}