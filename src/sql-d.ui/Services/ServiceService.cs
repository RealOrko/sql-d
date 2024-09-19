using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlD.Builders;
using SqlD.Configs.Model;
using SqlD.Logging;
using SqlD.Network;
using SqlD.UI.Models.Registry;
using SqlD.UI.Models.Services;

namespace SqlD.UI.Services;

public class ServiceService
{
    private readonly RegistryService registry;

    public ServiceService(RegistryService registryService)
    {
        registry = registryService;
    }

    public async Task<RegistryViewModel> GetRegistry()
    {
        return await registry.GetServices();
    }

    public void CreateService(ServiceFormViewModel service)
    {
        var config = Configs.Configuration.Instance;

        var sqlDServiceModel = new SqlDServiceModel
        {
            Name = service.Name,
            Database = service.Database,
            Host = service.Host,
            Port = service.Port,
            Tags = (service.Tags ?? string.Empty).Split(',').ToList()
        };

        var registryEntryViewModels = service.Forwards.Where(x => x.Selected).ToList();
        if (registryEntryViewModels.Any())
            sqlDServiceModel.ForwardingTo.AddRange(registryEntryViewModels.Select(y => new SqlDForwardingModel
            {
                Host = y.Host,
                Port = y.Port
            }));

        config.Services.Add(sqlDServiceModel);

        Configs.Configuration.Update(config);
        Interface.Start();
    }

    public void UpdateService(ServiceFormViewModel serviceModel)
    {
        var service = Configs.Configuration.Instance.Services.First(x => x.IsEqualTo(new EndPoint(serviceModel.Host, serviceModel.Port)));
        
        service.Name = serviceModel.Name;
        service.Database = serviceModel.Database;
        service.Host = serviceModel.Host;
        service.Port = serviceModel.Port;
        service.Tags = serviceModel.Tags.Split(',').ToList();

        var registryEntryViewModels = serviceModel.Forwards.Where(x => x.Selected).ToList();
        if (registryEntryViewModels.Any())
        {
            service.ForwardingTo = new List<SqlDForwardingModel>();
            service.ForwardingTo.AddRange(registryEntryViewModels.Select(y => new SqlDForwardingModel
            {
                Host = y.Host,
                Port = y.Port
            }));
        }

        Configs.Configuration.Update(Configs.Configuration.Instance);

        Interface.Stop();
        Interface.Start();
    }

    public void RemoveService(string host, int port)
    {
        var hostToKill = new EndPoint(host, port);

        try
        {
            Log.Out.Info($"Sending remote kill command to {hostToKill.ToUrl()}");
            new NewClientBuilder(false).ConnectedTo(hostToKill).Kill();
        }
        catch (Exception err)
        {
            Log.Out.Error(err.Message);
            Log.Out.Error(err.StackTrace);
        }

        try
        {
            Log.Out.Info($"Removing {hostToKill.ToUrl()} from config");
            var config = Configs.Configuration.Instance;
            config.Services = config.Services.Where(x => !x.IsEqualTo(hostToKill)).ToList();
            config.Services.ForEach(x => x.ForwardingTo = x.ForwardingTo.Where(x => !x.IsEqualTo(hostToKill)).ToList());
            Configs.Configuration.Update(config);
        }
        catch (Exception err)
        {
            Log.Out.Error(err.Message);
            Log.Out.Error(err.StackTrace);
        }

        Interface.Stop();
        Interface.Start();
    }
}