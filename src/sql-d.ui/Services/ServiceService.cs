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
        {
            sqlDServiceModel.ForwardingTo.AddRange(registryEntryViewModels.Select(y => new SqlDForwardingModel
            {
                Host = y.Host,
                Port = y.Port
            }));
        }

        config.Services.Add(sqlDServiceModel);

        Configs.Configuration.Update(config);
        Interface.Start();
    }

    public void UpdateService(ServiceFormViewModel serviceModel)
    {
        var serviceEndPoint = new EndPoint(serviceModel.Host, serviceModel.Port);
        var service = Configs.Configuration.Instance.Services.First(x => x.IsEqualTo(serviceEndPoint));
        
        service.Name = serviceModel.Name;
        service.Database = serviceModel.Database;
        service.Host = serviceModel.Host;
        service.Port = serviceModel.Port;
        service.Tags = serviceModel.Tags.Split(',').ToList();
        
        var nonForwardModels = serviceModel.Forwards.Where(x => !x.Selected).ToList();
        if (nonForwardModels.Any())
        {
            foreach (var nonForwardModel in nonForwardModels)
            {
                var nonForwardEndPoint = new EndPoint(nonForwardModel.Host, nonForwardModel.Port);
                service.ForwardingTo = service.ForwardingTo.Where(x => !x.IsEqualTo(nonForwardEndPoint)).ToList();
            }
        }

        var forwardModels = serviceModel.Forwards.Where(x => x.Selected).ToList();
        if (forwardModels.Any())
        {
            service.ForwardingTo.AddRange(forwardModels.Select(y => new SqlDForwardingModel
            {
                Host = y.Host,
                Port = y.Port
            }));
        }
        
        var nonReturnModels = serviceModel.Forwards.Where(x => !x.Selected).ToList();
        if (nonReturnModels.Any())
        {
            foreach (var nonReturnModel in nonReturnModels)
            {
                var nonReturnEndPoint = new EndPoint(nonReturnModel.Host, nonReturnModel.Port);
                service = Configs.Configuration.Instance.Services.First(x => x.IsEqualTo(nonReturnEndPoint));
                service.ForwardingTo = service.ForwardingTo.Where(x => !x.IsEqualTo(serviceEndPoint)).ToList();
            }
        }
        
        var returnModels = serviceModel.Returns.Where(x => x.Selected).ToList();
        if (returnModels.Any())
        {
            foreach (var returnModel in returnModels)
            {
                var returnEndPoint = new EndPoint(returnModel.Host, returnModel.Port);
                service = Configs.Configuration.Instance.Services.First(x => x.IsEqualTo(returnEndPoint));
                if (!service.ForwardingTo.Any(x => x.IsEqualTo(serviceEndPoint)))
                {
                    service.ForwardingTo.Add(new SqlDForwardingModel()
                    {
                        Host = serviceEndPoint.Host,
                        Port = serviceEndPoint.Port,
                    });
                }
            }
        }
        else
        {
            var otherServiceModels = Configs.Configuration.Instance.Services.Where(x => !x.IsEqualTo(serviceEndPoint));
            foreach (var otherServiceModel in otherServiceModels)
            {
                otherServiceModel.ForwardingTo = otherServiceModel.ForwardingTo.Where(x => !x.IsEqualTo(serviceEndPoint)).ToList();
            }
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
            Log.Out.Info($"Sending remote unregister command to {hostToKill.ToUrl()}");
            new NewClientBuilder(true).ConnectedTo(hostToKill).Unregister();
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