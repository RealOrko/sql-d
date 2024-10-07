using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using SqlD.Builders;
using SqlD.Configs;
using SqlD.Configs.Model;
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

        if (!Configuration.Instance.Settings.Forwarding.Allowed)
        {
            Log.Out.Info("Forwarding is disabled. Please enable this is you want near real-time replication with low data volumes for each transaction.");
        }
        
        context.Request.EnableBuffering();
        
        if (Configuration.Instance.Settings.Forwarding.Allowed && Configuration.Instance.Settings.Forwarding.Strategy == SqlDSettingsForwarding.SECONDARY_STRATEGY)
        {
            streamReader = await ExecuteForwards(context, SqlDSettingsForwarding.SECONDARY_STRATEGY);
        }

        try
        {
            await next.Invoke();
        }
        catch (Exception)
        {
            Log.Out.Warn("The middleware target could be disposed ... ");            
        }

        if (Configuration.Instance.Settings.Forwarding.Allowed && Configuration.Instance.Settings.Forwarding.Strategy == SqlDSettingsForwarding.PRIMARY_STRATEGY)
        {
            streamReader = await ExecuteForwards(context, SqlDSettingsForwarding.PRIMARY_STRATEGY);
        }

        if (streamReader != null)
        {
            streamReader.Dispose();
        }
    }

    private async Task<StreamReader> ExecuteForwards(HttpContext context, string strategy)
    {
        StreamReader streamReader = null;
        
        if (Configuration.Instance.FindForwardingAddresses(_listener.ServiceModel).Any())
        {
            if (strategy == SqlDSettingsForwarding.PRIMARY_STRATEGY)
            {
                context.Request.Body.Position = 0;
            }

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

            if (strategy == SqlDSettingsForwarding.SECONDARY_STRATEGY)
            {
                context.Request.Body.Position = 0;
            }
        }

        return streamReader;
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

    private static async Task<T> Deserialise<T>(StreamReader bodyRequeStreamReader)
    {
        var body = await bodyRequeStreamReader.ReadToEndAsync();
        var request = JsonConvert.DeserializeObject<T>(body);
        return request;
    }
}