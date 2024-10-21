using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SqlD.Configs.Model;
using SqlD.Network;
using SqlD.UI.Models.Registry;

namespace SqlD.UI.Models.Services;

public class ServiceFormViewModel
{
    private List<RegistryEntryViewModel> _forwards;
    private List<RegistryEntryViewModel> _returns;

    public ServiceFormViewModel()
    {
        _forwards = new List<RegistryEntryViewModel>();
        _returns = new List<RegistryEntryViewModel>();
    }

    public ServiceFormViewModel(List<RegistryEntryViewModel> registryEntries)
    {
        Name = Guid.NewGuid().ToString("N").Substring(0, 8);
        Host = "localhost";
        Port = registryEntries.Max(x => x.Port) + 1;
        Database = $"{Name}.db";
        Tags = Name;

        Forwards = registryEntries.Select(x => x.Clone()).ToList();
        Returns = registryEntries.Select(x => x.Clone()).ToList();
    }

    public ServiceFormViewModel(SqlDServiceModel serviceModel, List<RegistryEntryViewModel> registryEntries)
    {
        Name = serviceModel.Name;
        Host = serviceModel.Host;
        Port = serviceModel.Port;
        Database = serviceModel.Database;
        Tags = string.Join(", ", serviceModel.Tags);
        
        var registryFiltered = registryEntries.Where(x => !x.EndPoint.IsEqualTo(serviceModel));
        Forwards = registryFiltered.Select(x => x.Clone()).ToList();
        Returns = registryFiltered.Select(x => x.Clone()).ToList();
    }

    [Required]
    [DisplayName("Service Name")]
    public string Name { get; set; }

    [Required]
    [DisplayName("Service Host")]
    public string Host { get; set; }

    [Required]
    [Range(1024, 65535)]
    [DisplayName("Service Port")]
    public int Port { get; set; }

    [Required]
    [DisplayName("Database Name")]
    public string Database { get; set; }

    [Required] 
    [DisplayName("Tags")] 
    public string Tags { get; set; }
    
    public List<RegistryEntryViewModel> Forwards
    {
        get => _forwards.OrderBy(x => x.Tags).ToList();
        set => _forwards = value;
    }
    
    public List<RegistryEntryViewModel> Returns
    {
        get => _returns.OrderBy(x => x.Tags).ToList();
        set => _returns = value;
    }

    public EndPoint ToEndPoint()
    {
        return new EndPoint(Host, Port);
    }
}