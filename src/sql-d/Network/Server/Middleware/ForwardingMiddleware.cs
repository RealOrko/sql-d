using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using SqlD.Network.Client;
using SqlD.Network.Server.Api.Db.Model;

namespace SqlD.Network.Server.Middleware
{
	public class ForwardingMiddleware
	{
		private readonly ConnectionListener _listener;

		private List<EndPoint> _forwards
		{
			get
			{
				Configs.Configuration.ConfigReady.WaitOne();
				return Configs.Configuration.Instance.FindForwardingAddresses(_listener.EndPoint);
			}
		} 

		public ForwardingMiddleware(ConnectionListener listener)
		{
			_listener = listener;
		}

		public async Task InvokeAsync(HttpContext context, Func<Task> next)
		{
			StreamReader streamReader = null;
			if (_forwards.Any())
			{
				BeforeInvoke_BeforeRequestRead(context);

				try
				{
					if (context.Request.GetDisplayUrl().ToLower().Contains("/api/db/command"))
					{
						streamReader = new StreamReader(context.Request.Body);
						var commandRequest = await Deserialise<Command>(streamReader);
						await ForwardToClients(async client => await client.PostCommandAsync(commandRequest));
					}

					if (context.Request.GetDisplayUrl().ToLower().Contains("/api/db/scalar"))
					{
						streamReader = new StreamReader(context.Request.Body);
						var commandRequest = await Deserialise<Command>(streamReader);
						await ForwardToClients(async client => await client.PostScalarAsync(commandRequest));
					}
				}
				catch (Exception err)
				{
					Logging.Log.Out.Error(err.ToString());
				}

				BeforeInvoke_AfterRequestRead(context);
			}
			await next.Invoke();
			streamReader?.Dispose();
		}

		private async Task ForwardToClients(Func<ConnectionClient, Task<CommandResponse>> clientApiCall)
		{
			foreach (var forwardAddress in _forwards)
			{
				try
				{
					var client = Interface.NewClient().ConnectedTo(forwardAddress);

					var commandResponse = await clientApiCall(client);
					if (commandResponse.StatusCode != StatusCode.Ok)
					{
						Logging.Log.Out.Error($"Replicated to {forwardAddress} with {commandResponse.Error}");
					}
				}
				catch (Exception err)
				{
					Logging.Log.Out.Error(err.ToString());
				}
			}
		}

		public void BeforeInvoke_BeforeRequestRead(HttpContext context)
		{
			context.Request.EnableBuffering();
		}

		public void BeforeInvoke_AfterRequestRead(HttpContext context)
		{
			context.Request.Body.Position = 0;
		}

		private static async Task<T> Deserialise<T>(StreamReader bodyRequeStreamReader)
		{
			var body = await bodyRequeStreamReader.ReadToEndAsync();
			var request = JsonConvert.DeserializeObject<T>(body);
			return request;
		}
	}
}