using System.Net;
using SqlD.Configs.Model;
using SqlD.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace SqlD.Network.Server;

public class ConnectionListener : IDisposable
{
    private IHost _host;

    static ConnectionListener()
    {
        ServicePointManager.UseNagleAlgorithm = true;
    }

    internal ConnectionListener()
    {
    }

    public DbConnectionFactory DbConnectionFactory { get; private set; }
    public SqlDServiceModel ServiceModel { get; private set; }

    internal virtual void Listen(SqlDServiceModel serviceModel, DbConnectionFactory listenerDbConnectionFactory)
    {
        ServiceModel = serviceModel;
        DbConnectionFactory = listenerDbConnectionFactory;
        ConnectionListenerStartup.Listener = this;

        try
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<ConnectionListenerStartup>();
                    builder.ConfigureKestrel(opts =>
                    {
                        opts.AddServerHeader = true;
                        opts.Limits.MaxRequestBodySize = int.MaxValue;
                        opts.Limits.MaxResponseBufferSize = int.MaxValue;
                        opts.Limits.MaxConcurrentConnections = int.MaxValue;
                        opts.ListenAnyIP(ServiceModel.Port);
                    });
                    builder.UseUrls(ServiceModel.ToWildcardUrl());
                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole(opts => { opts.LogToStandardErrorThreshold = LogLevel.Error; });
                }).Build();

            _host.Start();

            Log.Out.Info($"Started connection listener on {ServiceModel.ToUrl()}");
        }
        catch (Exception err)
        {
            Log.Out.Error($"Failed to listen on {ServiceModel.ToUrl()}, {err}");
            throw err;
        }
    }
    
    public virtual void Dispose()
    {
        _host.StopAsync().Wait();
        _host.Dispose();

        Log.Out.Info($"Disposed connection listener on {ServiceModel.ToUrl()}");
    }
}