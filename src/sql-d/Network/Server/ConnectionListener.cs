using System.Net;
using SqlD.Configs.Model;
using SqlD.Logging;

namespace SqlD.Network.Server;

public class ConnectionListener : IDisposable
{
    private IHost host;

    static ConnectionListener()
    {
        ServicePointManager.UseNagleAlgorithm = true;
    }

    internal ConnectionListener()
    {
    }

    public DbConnection DbConnection { get; private set; }
    public SqlDServiceModel ServiceModel { get; private set; }

    public virtual void Dispose()
    {
        host.StopAsync().Wait();
        host.Dispose();

        Log.Out.Info($"Disposed listener on {ServiceModel.ToUrl()}");
    }

    internal virtual void Listen(SqlDServiceModel serviceModel, DbConnection listenerDbConnection)
    {
        serviceModel = serviceModel ?? throw new ArgumentNullException(nameof(serviceModel));
        listenerDbConnection = listenerDbConnection ?? throw new ArgumentNullException(nameof(listenerDbConnection));

        ServiceModel = serviceModel;
        DbConnection = listenerDbConnection;
        ConnectionListenerStartup.Listener = this;

        try
        {
            host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<ConnectionListenerStartup>();
                    builder.ConfigureKestrel(opts =>
                    {
                        opts.AddServerHeader = true;
                        opts.Limits.MaxRequestBodySize = null;
                        opts.Limits.MaxResponseBufferSize = null;
                        opts.Limits.MaxConcurrentConnections = null;
                        opts.ListenAnyIP(ServiceModel.Port);
                    });
                    builder.UseUrls(ServiceModel.ToWildcardUrl());
                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole(opts => { opts.LogToStandardErrorThreshold = LogLevel.Error; });
                }).Build();

            host.Start();

            Log.Out.Info($"Connection listener on {ServiceModel.ToUrl()}");
        }
        catch (Exception err)
        {
            Log.Out.Error($"Failed to listen on {ServiceModel.ToUrl()}, {err}");
            throw err;
        }
    }
}