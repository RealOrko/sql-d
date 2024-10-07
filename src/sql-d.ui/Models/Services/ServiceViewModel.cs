using System.Collections.Generic;
using SqlD.Configs.Model;
using SqlD.UI.Models.Registry;

namespace SqlD.UI.Models.Services;

public class ServiceViewModel
{
    public ServiceViewModel(SqlDConfiguration config, List<RegistryEntryViewModel> registryEntries)
    {
        Config = config;
        RegistryEntries = registryEntries;
    }

    public SqlDConfiguration Config { get; set; }
    public List<RegistryEntryViewModel> RegistryEntries { get; set; }

    public bool ContainsConfig(RegistryEntryViewModel service)
    {
        foreach (var configService in Config.Services)
            if (service.EndPoint.Equals(configService))
                return true;

        return false;
    }
}