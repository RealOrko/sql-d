using SqlD.Logging;
using SqlD.Network;

namespace SqlD.Configs.Model;

public class SqlDConfiguration
{
    public bool Enabled { get; set; } = true;

    public string DataDirectory { get; set; } = null;

    public List<SqlDServiceModel> Services { get; set; } = new();
    public List<SqlDRegistryModel> Registries { get; set; } = new();

    public static SqlDConfiguration Default()
    {
        return new SqlDConfiguration
        {
            Enabled = true,
            Registries = new List<SqlDRegistryModel>(),
            Services = new List<SqlDServiceModel>()
        };
    }
    
    public void SetDataDirectory(string dataDirectory)
    {
        if (!string.IsNullOrWhiteSpace(dataDirectory))
        {
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