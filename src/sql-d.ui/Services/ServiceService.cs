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

    public void CreateService(ServiceFormViewModel serviceFormModel)
    {
        var config = Configs.Configuration.Instance;

        var serviceModel = new SqlDServiceModel
        {
            Name = serviceFormModel.Name,
            Database = serviceFormModel.Database,
            Host = serviceFormModel.Host,
            Port = serviceFormModel.Port,
            Tags = (serviceFormModel.Tags ?? string.Empty).Split(',').ToList()
        };
        
        var serviceEndPoint = new EndPoint(serviceModel.Host, serviceModel.Port);
        
        var registryEntryViewModels = serviceFormModel.Forwards.Where(x => x.Selected).ToList();
        if (registryEntryViewModels.Any())
        {
            serviceModel.ForwardingTo.AddRange(registryEntryViewModels.Select(y => new SqlDForwardingModel
            {
                Host = y.Host,
                Port = y.Port
            }));
        }

        config.Services.Add(serviceModel);
        
        EnsureModelForSerivceConfiguration(serviceFormModel, serviceModel, serviceEndPoint);

        Configs.Configuration.Update(config);
        Interface.Start();
    }

    public void UpdateService(ServiceFormViewModel serviceFormModel)
    {
        var serviceEndPoint = new EndPoint(serviceFormModel.Host, serviceFormModel.Port);
        var service = Configs.Configuration.Instance.Services.First(x => x.IsEqualTo(serviceEndPoint));
        
        service.Name = serviceFormModel.Name;
        service.Database = serviceFormModel.Database;
        service.Host = serviceFormModel.Host;
        service.Port = serviceFormModel.Port;
        service.Tags = serviceFormModel.Tags.Split(',').ToList();
        
        EnsureModelForSerivceConfiguration(serviceFormModel, service, serviceEndPoint);

        Configs.Configuration.Update(Configs.Configuration.Instance);

        Interface.Stop();
        Interface.Start();
    }

    private static void EnsureModelForSerivceConfiguration(ServiceFormViewModel serviceModel, SqlDServiceModel service, EndPoint serviceEndPoint)
    {
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