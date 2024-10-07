using Microsoft.AspNetCore.Mvc;
using SqlD.Logging;
using SqlD.Network.Server.Api.Unregister.Model;

namespace SqlD.Network.Server.Api.Unregister.Controllers;

[ApiController]
[Route("api/unregister")]
public class UnregisterController : Controller
{
    private readonly EndPoint authorityAddress;

    public UnregisterController(EndPoint serverAddress)
    {
        authorityAddress = serverAddress;
    }

    [HttpPost]
    public IActionResult Unregister([FromBody] UnregisterRequest request)
    {
        return this.Intercept(() =>
        {
            Log.Out.Warn($"Unregistering {request.EndPoint.ToUrl()}, sending unregister event ... ");
            Registry.Registry.Unregister(request.EndPoint);
            return Ok(new UnregisterResponse(authorityAddress));
        });
    }
}