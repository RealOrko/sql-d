using System.Collections.Concurrent;
using SqlD.Builders;

namespace SqlD.Network.Server.Workers;

public class SynchronisationWorker : IHostedService
{
    private readonly EndPoint _listenerEndPoint;
    private readonly DbConnectionFactory _dbConnectionFactory;
    internal static ConcurrentQueue<EndPoint> SyncronisationTasks { get; } = new();

    public SynchronisationWorker(EndPoint listenerEndPoint, DbConnectionFactory dbConnectionFactory)
    {
        _listenerEndPoint = listenerEndPoint;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Factory.StartNew(async () => await Synchronise(cancellationToken), TaskCreationOptions.LongRunning);
    }

    private async Task Synchronise(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            if (SyncronisationTasks.TryDequeue(out var endPoint))
            {
                await GetDatabaseFrom(endPoint);
            }
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }
    
    private async Task GetDatabaseFrom(EndPoint endPoint)
    {
        using (var dbConnection = _dbConnectionFactory.Connect())
        {
            var forwardingClient = new NewClientBuilder(true).ConnectedTo(endPoint);
            await forwardingClient.DownloadDatabaseTo(dbConnection.GetDatabaseFilePath());
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
}