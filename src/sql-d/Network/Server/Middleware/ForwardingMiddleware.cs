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
        var forwardingStrategy = Configuration.Instance.Settings.Forwarding.Strategy;
        
        if (!Configuration.Instance.Settings.Forwarding.Allowed)
        {
            Log.Out.Debug("Forwarding is disabled. Please enable this is you want near real-time replication with low data volumes for each write.");
        }
        else
        {
            if (Configuration.Instance.Settings.Forwarding.Allowed && forwardingStrategy == SqlDSettingsForwarding.PRIMARY_STRATEGY)
            {
                Log.Out.Debug($"Forwarding Strategy={forwardingStrategy}. Source forwarding node will be updated before the read replicas. Replicas will read lag for writes but writing large volumes of data result in a performance penalty, for this you should consider switching off forwarding entirely and using replication only, this always has read lag depending on the replication interval.");
            }
            if (Configuration.Instance.Settings.Forwarding.Allowed && forwardingStrategy == SqlDSettingsForwarding.SECONDARY_STRATEGY)
            {
                Log.Out.Debug($"Forwarding Strategy={forwardingStrategy}. Read replicas will be updated before the source forwarding node. Replicas will not have read lag for writes but writing large volumes of data result in a performance penalty, for this you should consider switching off forwarding entirely and using replication only, this always has read lag depending on the replication interval.");
            }
        }
        
        context.Request.EnableBuffering();
        
        if (Configuration.Instance.Settings.Forwarding.Allowed && forwardingStrategy == SqlDSettingsForwarding.SECONDARY_STRATEGY)
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

        if (Configuration.Instance.Settings.Forwarding.Allowed && forwardingStrategy == SqlDSettingsForwarding.PRIMARY_STRATEGY)
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
        {
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
    }

    private static async Task<T> Deserialise<T>(StreamReader bodyRequeStreamReader)
    {
        var body = await bodyRequeStreamReader.ReadToEndAsync();
        var request = JsonConvert.DeserializeObject<T>(body);
        return request;
    }
}