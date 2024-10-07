using System;
using Microsoft.AspNetCore.Http;
using SqlD.Network;
using SqlD.Network.Server.Api.Registry.Model;

namespace SqlD.UI.Models.Registry;

public class RegistryEntryViewModel
{
    public RegistryEntryViewModel()
    {
        Uri = string.Empty;
        Name = string.Empty;
        Database = string.Empty;
        AuthorityUri = string.Empty;
        State = string.Empty;
        Tags = string.Empty;
        LastUpdated = DateTime.UtcNow;
        EndPoint = new EndPoint();
    }

    public RegistryEntryViewModel(RegistryEntry entry)
    {
        Uri = entry.Uri;
        Name = entry.Name;
        Database = entry.Database;
        AuthorityUri = entry.AuthorityUri;
        Tags = string.Join(", ", entry.TagsAsArray);
        LastUpdated = entry.LastUpdated;
        EndPoint = EndPoint.FromUri(entry.Uri);
    }

    public DateTime LastUpdated { get; set; }
    public string Tags { get; set; }
    public string Name { get; set; }
    public string State { get; set; }
    public string Database { get; set; }
    public string AuthorityUri { get; set; }
    public string Uri { get; set; }
    public EndPoint EndPoint { get; set; }
    public bool Selected { get; set; }

    public string Host
    {
        get => EndPoint.Host;
        set => EndPoint.Host = value;
    }

    public int Port
    {
        get => EndPoint.Port;
        set => EndPoint.Port = value;
    }

    public RegistryEntryViewModel Clone()
    {
        var cloned = new RegistryEntryViewModel();
        cloned.Uri = Uri;
        cloned.Name = Name;
        cloned.Database = Database;
        cloned.AuthorityUri = AuthorityUri;
        cloned.State = State.Clone().ToString();
        cloned.Tags = Tags.Clone().ToString();
        cloned.LastUpdated = LastUpdated;
        cloned.EndPoint = new EndPoint(EndPoint.Host, EndPoint.Port);
        return cloned;
    }

    public string GetRedirectedUri(HttpRequest request)
    {
        if (Host.ToLower() == "localhost") return $"{request.Scheme}://{request.Host.Host}:{Port}/";

        return EndPoint.ToUrl();
    }

    public string GetRedirectedAuthorityUri(HttpRequest request)
    {
        var endPoint = EndPoint.FromUri(AuthorityUri);
        if (endPoint.Host.ToLower() == "localhost") return $"{request.Scheme}://{request.Host.Host}:{endPoint.Port}/";

        return endPoint.ToUrl();
    }
}