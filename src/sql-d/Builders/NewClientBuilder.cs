using SqlD.Network;
using SqlD.Network.Client;

namespace SqlD.Builders;

internal class NewClientBuilder
{
    private readonly int httpClientTimeoutMilliseconds;
    private readonly int retryLimit;
    private readonly bool withRetries;

    internal NewClientBuilder(bool withRetries, int retryLimit = 50, int httpClientTimeoutMilliseconds = 120000)
    {
        this.withRetries = withRetries;
        this.retryLimit = retryLimit;
        this.httpClientTimeoutMilliseconds = httpClientTimeoutMilliseconds;
    }

    public ConnectionClient ConnectedTo(EndPoint endPoint)
    {
        return ConnectionClientFactory.Create(endPoint, withRetries, retryLimit, httpClientTimeoutMilliseconds);
    }
}