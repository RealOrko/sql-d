using SqlD.Builders;

namespace SqlD.Network.Server.Workers;

public class SynchronisationWorker : IHostedService
{
    private readonly EndPoint _listenerEndPoint;
    private readonly DbConnectionFactory _dbConnectionFactory;

    public SynchronisationWorker(EndPoint listenerEndPoint, DbConnectionFactory dbConnectionFactory)
    {
        _listenerEndPoint = listenerEndPoint;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var forwardingServices = Configs.Configuration.Instance.Services.Where(x => x.ForwardingTo.Any(y => y.IsEqualTo(_listenerEndPoint)));
        if (forwardingServices.Any())
        {
            using (var dbConnection = _dbConnectionFactory.Connect())
            {
                foreach (var forwardingService in forwardingServices)
                {
                    var forwardingClient = new NewClientBuilder(true).ConnectedTo(forwardingService);
                    await forwardingClient.DownloadDatabaseTo(dbConnection.GetDatabaseFilePath());
                }
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
}