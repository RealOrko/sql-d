using System.Collections.Concurrent;
using SqlD.Builders;
using SqlD.Logging;

namespace SqlD.Network.Server.Workers;

public class SynchronisationWorkerQueue
{
    public ConcurrentQueue<EndPoint> SyncronisationTasks { get; } = new();
}

public class SynchronisationWorker : IHostedService
{
    private readonly EndPoint _listenerEndPoint;
    private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly SynchronisationWorkerQueue _queue;

    public SynchronisationWorker(EndPoint listenerEndPoint, DbConnectionFactory dbConnectionFactory, SynchronisationWorkerQueue queue)
    {
        _listenerEndPoint = listenerEndPoint;
        _dbConnectionFactory = dbConnectionFactory;
        _queue = queue;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Factory.StartNew(async () => await Synchronise(cancellationToken), TaskCreationOptions.LongRunning);
    }

    private async Task Synchronise(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            if (_queue.SyncronisationTasks.TryDequeue(out var endPoint))
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
            Log.Out.Info($"Database successfully written to {dbConnection.GetDatabaseFilePath()} ... ");
            Log.Out.Info($"Successfully synchronised database from {endPoint.ToUrl()} to {_listenerEndPoint.ToUrl()} ... ");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
}