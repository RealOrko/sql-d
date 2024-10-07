using System.Collections.Concurrent;
using SqlD.Builders;
using SqlD.Logging;

namespace SqlD.Network.Server.Workers;

public class AutoSynchronisationWorker : IHostedService
{
    private readonly EndPoint _listenerEndPoint;
    private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly SynchronisationWorkerQueue _queue;

    private TimeSpan _replicationInterval
    {
        get { return TimeSpan.FromSeconds(Configs.Configuration.Instance.Replication.Interval); }
    }

    public AutoSynchronisationWorker(EndPoint listenerEndPoint, DbConnectionFactory dbConnectionFactory, SynchronisationWorkerQueue queue)
    {
        _listenerEndPoint = listenerEndPoint;
        _dbConnectionFactory = dbConnectionFactory;
        _queue = queue;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Log.Out.Info($"Replication is enabled with an interval of {Configs.Configuration.Instance.Replication.Interval} second(s).");
        await Task.Factory.StartNew(async () => await Synchronise(cancellationToken), TaskCreationOptions.LongRunning);
    }

    private async Task Synchronise(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        while (cancellationToken.IsCancellationRequested == false)
        {
            var forwardServiceModels = Configs.Configuration.Instance.Services.Where(x => x.ForwardingTo.Any(y => y.IsEqualTo(_listenerEndPoint)));
            if (forwardServiceModels.Any())
            {
                foreach (var forwardServiceModel in forwardServiceModels)
                {
                    var client = new NewClientBuilder(true).ConnectedTo(forwardServiceModel);
                    var upstreamHash = await client.SynchroniseHash();
                    var downstreamHash = string.Empty;
                    using (var dbConnection = _dbConnectionFactory.Connect())
                    {
                        downstreamHash = dbConnection.GetDatabaseFileHash();
                    }
                    if (upstreamHash != downstreamHash)
                    {
                        _queue.SyncronisationTasks.Enqueue(forwardServiceModel);
                    }
                }
            }
            await Task.Delay(_replicationInterval, cancellationToken);
        }
        Log.Out.Info($"Replication has stopped because the cancellation token was fired.");
    }
    
    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
}