using System.Collections.Concurrent;
using SqlD.Builders;
using SqlD.Logging;

namespace SqlD.Network.Server.Workers;

public class AutoSynchronisationWorker(EndPoint listenerEndPoint, DbConnectionFactory dbConnectionFactory, SynchronisationWorkerQueue queue) : IHostedService
{
    private static TimeSpan ReplicationInterval => TimeSpan.FromSeconds(Configs.Configuration.Instance.Settings.Replication.Interval);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Factory.StartNew(async () => await Synchronise(cancellationToken), TaskCreationOptions.LongRunning);
    }

    private async Task Synchronise(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        while (cancellationToken.IsCancellationRequested == false)
        {
            if (!Configs.Configuration.Instance.Settings.Replication.Allowed) continue;
            Log.Out.Info($"Replication is enabled with an interval of {Configs.Configuration.Instance.Settings.Replication.Interval} second(s).");
            try
            {
                var forwardServiceModels = Configs.Configuration.Instance.Services.Where(x => x.ForwardingTo.Any(y => y.IsEqualTo(listenerEndPoint)));
                if (forwardServiceModels.Any())
                {
                    foreach (var forwardServiceModel in forwardServiceModels)
                    {
                        var client = new NewClientBuilder(true).ConnectedTo(forwardServiceModel);
                        var upstreamHash = await client.SynchroniseHash();
                        var downstreamHash = string.Empty;
                        using (var dbConnection = dbConnectionFactory.Connect())
                        {
                            downstreamHash = dbConnection.GetDatabaseFileHash();
                        }

                        if (upstreamHash != downstreamHash)
                        {
                            queue.SyncronisationTasks.Enqueue(forwardServiceModel);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Log.Out.Error($"An error occured while synchronising a replication target: {err.Message}");
            }
            await Task.Delay(ReplicationInterval, cancellationToken);
        }
        Log.Out.Info($"Replication has stopped because the cancellation token was fired.");
    }
    
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.Factory.StartNew(() => Task.CompletedTask, cancellationToken);
    }
}