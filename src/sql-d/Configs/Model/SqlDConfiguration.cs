using SqlD.Logging;
using SqlD.Network;

namespace SqlD.Configs.Model;

public class SqlDConfiguration
{
    public string LogLevel { get; set; } = "info";
    public bool Enabled { get; set; } = true;
    public string DataDirectory { get; set; } = null;
    public SqlDSettings Settings { get; set; } = new();
    public List<SqlDServiceModel> Services { get; set; } = new();
    public List<SqlDRegistryModel> Registries { get; set; } = new();
    
    public void SetDataDirectory(string dataDirectory)
    {
        if (!string.IsNullOrWhiteSpace(dataDirectory) && string.IsNullOrEmpty(DataDirectory))
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLD_DATA_DIRECTORY")))
                dataDirectory = Environment.GetEnvironmentVariable("SQLD_DATA_DIRECTORY");
            
            Log.Out.Info($"Setting data directory to {dataDirectory}");
            DataDirectory = dataDirectory;
        }
    }
    
    public IEnumerable<EndPoint> FindForwardingAddresses(EndPoint listenerEndpoint)
    {
        var service = Services.FirstOrDefault(x => x.IsEqualTo(listenerEndpoint));
        if (service == null)
        {
            Log.Out.Warn($"Tried to find forwarding addresses for {listenerEndpoint.ToUrl()} but the service does not exist in configuration.");
            return [];
        }
        return service.ForwardingTo;
    }

    public override string ToString()
    {
        return $"{nameof(Enabled)}: {Enabled}, {nameof(Services)}: {Services}, {nameof(Registries)}: {Registries}";
    }
}