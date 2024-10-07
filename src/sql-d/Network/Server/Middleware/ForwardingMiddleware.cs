using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using SqlD.Builders;
using SqlD.Configs;
using SqlD.Logging;
using SqlD.Network.Client;
using SqlD.Network.Server.Api.Db.Model;

namespace SqlD.Network.Server.Middleware;

public class ForwardingMiddleware
{
    private readonly ConnectionListener _listener;

    public ForwardingMiddleware(ConnectionListener listener)
    {
        _listener = listener;
    }

    public async Task InvokeAsync(HttpContext context, Func<Task> next)
    {
        StreamReader streamReader = null;
        if (Configuration.Instance.FindForwardingAddresses(_listener.ServiceModel).Any())
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
                Log.Out.Error(err.ToString());
            }

            BeforeInvoke_AfterRequestRead(context);
        }

        await next.Invoke();
        streamReader?.Dispose();
    }

    private async Task ForwardToClients(Func<ConnectionClient, Task<CommandResponse>> clientApiCall)
    {
        foreach (var forwardAddress in Configuration.Instance.FindForwardingAddresses(_listener.ServiceModel))
            try
            {
                var client = new NewClientBuilder(true).ConnectedTo(forwardAddress);

                var commandResponse = await clientApiCall(client);
                if (commandResponse.StatusCode != StatusCode.Ok) Log.Out.Error($"Replicated to {forwardAddress} with {commandResponse.Error}");
            }
            catch (Exception err)
            {
                Log.Out.Error(err.ToString());
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