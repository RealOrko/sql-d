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
        var service = new SqlDServiceModel
        {
            Name = serviceFormModel.Name,
            Database = serviceFormModel.Database,
            Host = serviceFormModel.Host,
            Port = serviceFormModel.Port,
            Tags = (serviceFormModel.Tags ?? string.Empty).Split(',').ToList()
        };
        
        Configs.Configuration.Instance.Services.Add(service);
        
        EnsureModelForServiceConfiguration(serviceFormModel, service);

        Configs.Configuration.Update(Configs.Configuration.Instance);
        
        Interface.Start();
    }

    public void UpdateService(ServiceFormViewModel serviceFormModel)
    {
        var service = Configs.Configuration.Instance.Services.First(x => x.IsEqualTo(serviceFormModel.ToEndPoint()));
        
        service.Name = serviceFormModel.Name;
        service.Database = serviceFormModel.Database;
        service.Host = serviceFormModel.Host;
        service.Port = serviceFormModel.Port;
        service.Tags = serviceFormModel.Tags.Split(',').ToList();
        
        EnsureModelForServiceConfiguration(serviceFormModel, service);

        Configs.Configuration.Update(Configs.Configuration.Instance);
        
        Interface.Stop(Interface.InterfaceStopKind.NotFoundInConfig);
        Interface.Start();
    }

    public void RemoveService(string host, int port)
    {
        var serviceEndPoint = new EndPoint(host, port);

        new NewClientBuilder(true).ConnectedTo(serviceEndPoint).Unregister();

        Configs.Configuration.Instance.Services = Configs.Configuration.Instance.Services.Where(x => !x.IsEqualTo(serviceEndPoint)).ToList();
        Configs.Configuration.Instance.Services.ForEach(x => x.ForwardingTo = x.ForwardingTo.Where(x => !x.IsEqualTo(serviceEndPoint)).ToList());
        Configs.Configuration.Update(Configs.Configuration.Instance);

        Interface.Stop(Interface.InterfaceStopKind.NotFoundInConfig);
        Interface.Start();
    }
    
    private static void EnsureModelForServiceConfiguration(ServiceFormViewModel serviceModel, SqlDServiceModel service)
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
                service.ForwardingTo = service.ForwardingTo.Where(x => !x.IsEqualTo(service)).ToList();
            }
        }
        
        var returnModels = serviceModel.Returns.Where(x => x.Selected).ToList();
        if (returnModels.Any())
        {
            foreach (var returnModel in returnModels)
            {
                var returnEndPoint = new EndPoint(returnModel.Host, returnModel.Port);
                service = Configs.Configuration.Instance.Services.First(x => x.IsEqualTo(returnEndPoint));
                if (!service.ForwardingTo.Any(x => x.IsEqualTo(serviceModel.ToEndPoint())))
                {
                    service.ForwardingTo.Add(new SqlDForwardingModel()
                    {
                        Host = serviceModel.Host,
                        Port = serviceModel.Port,
                    });
                }
            }
        }
        else
        {
            var otherServiceModels = Configs.Configuration.Instance.Services.Where(x => !x.IsEqualTo(serviceModel.ToEndPoint()));
            foreach (var otherServiceModel in otherServiceModels)
            {
                otherServiceModel.ForwardingTo = otherServiceModel.ForwardingTo.Where(x => !x.IsEqualTo(serviceModel.ToEndPoint())).ToList();
            }
        }
    }

    public async Task SynchroniseForward(EndPoint thisEndPoint, EndPoint fromEndPoint)
    {
        await new NewClientBuilder(true).ConnectedTo(thisEndPoint).SynchroniseWith(fromEndPoint);
    }
    
    public async Task RemoveForward(EndPoint thisEndPoint, EndPoint fromEndPoint)
    {
        var serviceModel = Configs.Configuration.Instance.Services.First(x => x.IsEqualTo(fromEndPoint));
        serviceModel.ForwardingTo = serviceModel.ForwardingTo.Where(x => !x.IsEqualTo(thisEndPoint)).ToList();
        
        Configs.Configuration.Update(Configs.Configuration.Instance);

        Interface.Stop(Interface.InterfaceStopKind.NotFoundInConfig);
        Interface.Start();
    }

    public async Task SynchroniseForwardAll()
    {
        foreach (var fromEndPoint in Configs.Configuration.Instance.Services)
        {
            foreach (var thisEndPoint in fromEndPoint.ForwardingTo)
            {
                await SynchroniseForward(thisEndPoint, fromEndPoint);
            }
        }
    }
}