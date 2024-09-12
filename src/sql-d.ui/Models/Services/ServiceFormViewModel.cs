using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SqlD.Network;
using SqlD.UI.Models.Registry;

namespace SqlD.UI.Models.Services;

public class ServiceFormViewModel
{
    private List<RegistryEntryViewModel> _forwards;

    public ServiceFormViewModel()
    {
        Forwards = new List<RegistryEntryViewModel>();
    }

    public ServiceFormViewModel(List<RegistryEntryViewModel> registryEntries) : this()
    {
        var endPoints = registryEntries.Select(x => EndPoint.FromUri(x.Uri));

        Name = Guid.NewGuid().ToString("N");
        Host = endPoints.First().Host;
        Port = endPoints.Max(x => x.Port) + 1;
        Database = $"{Name}.db";

        if (registryEntries.Count == 1)
            Tags = "master";
        else
            Tags = string.Empty;

        Forwards = registryEntries;
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

    [Required] [DisplayName("Tags")] public string Tags { get; set; }

    public List<RegistryEntryViewModel> Forwards
    {
        get => _forwards.OrderBy(x => x.Tags).ToList();
        set => _forwards = value;
    }
}