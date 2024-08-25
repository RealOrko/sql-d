using Microsoft.AspNetCore.Mvc;
using SqlD.Configuration.Model;
using SqlD.Logging;
using SqlD.Network.Server.Api.Kill.Model;

namespace SqlD.Network.Server.Api.Kill.Controllers
{
    [ApiController]
	[Route("api/kill")]
	public class KillController : Controller
	{
		private readonly EndPoint authorityAddress;

		public KillController(EndPoint serverAddress, SqlDConfiguration configuration)
		{
			this.authorityAddress = new EndPoint(configuration.Authority, serverAddress.Port); ;
		} 

		[HttpPost]
		public IActionResult Kill([FromBody] KillRequest request)
		{
			return this.Intercept(() =>
			{
				Log.Out.Warn($"Unregistering {request.EndPoint.ToUrl()}, this process is going down ... ");
				Registry.Registry.Unregister(request.EndPoint);

			    var connectionListener = ConnectionListenerFactory.Find(request.EndPoint);
			    if (connectionListener != null)
			    {
			        ConnectionListenerFactory.Dispose(connectionListener);
			        Log.Out.Info($"Successfully killed list for endpoint {request.EndPoint.ToUrl()}");
			    }
			    else
			    {
				    Log.Out.Warn($"Could not find listener for {request.EndPoint.ToUrl()}");
			    }

                return Ok(new KillResponse(authorityAddress));
			});
		}
	}
}