namespace SqlD.Network;

public class EndPoint
{
    public EndPoint()
    {
    }

    public EndPoint(string host, int port)
    {
        Host = host;
        Port = port;
    }

    public string Host { get; set; }

    public int Port { get; set; }

    public string ToUrl(string resource = null)
    {
        return $"http://{Host}:{Port}/{resource}";
    }

    public string ToWildcardUrl()
    {
        return $"http://*:{Port}";
    }

    public static EndPoint FromUri(string uri)
    {
        if (string.IsNullOrEmpty(uri))
            return null;

        var _uri = new Uri(uri);
        return new EndPoint(_uri.Host, _uri.Port);
    }

    public bool IsEqualTo(EndPoint endPoint)
    {
        if(endPoint == null) return false;
        if(endPoint.Host != Host) return false;
        if(endPoint.Port != Port) return false;
        return true;
    }

    public override string ToString()
    {
        return $"{nameof(Host)}: {Host}, {nameof(Port)}: {Port}";
    }
}