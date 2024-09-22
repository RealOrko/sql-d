namespace SqlD.Network;

internal class UrlBuilder
{
    internal static string GetPingUrl(EndPoint endpoint)
    {
        return endpoint.ToUrl("api/ping");
    }

    internal static string GetScalarUrl(EndPoint endpoint)
    {
        return endpoint.ToUrl("api/db/scalar");
    }

    internal static string GetCommandUrl(EndPoint endpoint)
    {
        return endpoint.ToUrl("api/db/command");
    }

    internal static string GetDescribeUrl(EndPoint endpoint)
    {
        return endpoint.ToUrl("api/db/describe");
    }

    internal static string GetQueryUrl(EndPoint endPoint)
    {
        return endPoint.ToUrl("api/db/query");
    }

    internal static string GetFileStreamUrl(EndPoint endPoint)
    {
        return endPoint.ToUrl("api/db/file");
    }

    internal static string GetRegistryUrl(EndPoint endPoint)
    {
        return endPoint.ToUrl("api/registry");
    }

    public static string GetUnregisterUrl(EndPoint endPoint)
    {
        return endPoint.ToUrl("api/unregister");
    }
}