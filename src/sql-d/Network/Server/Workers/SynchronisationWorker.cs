using System.Collections.Concurrent;
using SqlD.Builders;
using SqlD.Logging;

namespace SqlD.Network.Server.Workers;

public class SynchronisationWorkerQueue
{
    public ConcurrentQueue<EndPoint> SyncronisationTasks { get; } = new();
}

public class SynchronisationWorker(EndPoint listenerEndPoint, DbConnectionFactory dbConnectionFactory, SynchronisationWorkerQueue queue) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Factory.StartNew(async () => await Synchronise(cancellationToken), TaskCreationOptions.LongRunning);
    }

    private async Task Synchronise(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            if (queue.SyncronisationTasks.TryDequeue(out var endPoint))
            {
                await GetDatabaseFrom(endPoint);
            }
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }
    
    private async Task GetDatabaseFrom(EndPoint endPoint)
    {
        using (var dbConnection = dbConnectionFactory.Connect())
        {
            var forwardingClient = new NewClientBuilder(true).ConnectedTo(endPoint);
            await forwardingClient.DownloadDatabaseTo(dbConnection.GetDatabaseFilePath());
            Log.Out.Debug($"Database successfully written to {dbConnection.GetDatabaseFilePath()} ... ");
            Log.Out.Info($"Successfully synchronised database from {endPoint.ToUrl()} to {listenerEndPoint.ToUrl()} ... ");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.Factory.StartNew(() => Task.CompletedTask, cancellationToken);
    }
}