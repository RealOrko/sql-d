using System.Collections;

namespace SqlD.Network.Server.Api.Id.Model;

public class IdResponse
{
    public IdResponse(EndPoint authorityAddress, HttpRequest request)
    {
        ServerDateTime = DateTime.UtcNow;
        AuthorityAddress = GetRedirectedAuthorityUri(request, authorityAddress.ToUrl());
        RuntimeVersion = Environment.Version.ToString();
        EnvironmentVariables = SortByKey(Environment.GetEnvironmentVariables());
    }

    public string RuntimeVersion { get; }
    public EndPoint AuthorityAddress { get; }
    public DateTime ServerDateTime { get; }
    public IDictionary EnvironmentVariables { get; }

    private IDictionary SortByKey(IDictionary dictionary)
    {
        var keys = (from object keyValue in dictionary.Keys select keyValue.ToString()).ToList();
        keys.Sort();

        var results = new Dictionary<string, string>();
        foreach (var key in keys)
            results.Add(key, dictionary[key].ToString());

        return results;
    }

    public EndPoint GetRedirectedAuthorityUri(HttpRequest request, string authorityUri)
    {
        var endPoint = EndPoint.FromUri(authorityUri);
        if (endPoint.Host.ToLower() == "localhost") return EndPoint.FromUri($"{request.Scheme}://{request.Host.Host}:{endPoint.Port}/");

        return endPoint;
    }
}