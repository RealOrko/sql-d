using SqlD.Network;
using SqlD.Network.Client;

namespace SqlD.Builders;

internal class NewClientBuilder
{
    private readonly int _httpClientTimeoutMilliseconds;
    private readonly int _retryLimit;
    private readonly bool _withRetries;

    internal NewClientBuilder(bool withRetries, int retryLimit = 3, int httpClientTimeoutMilliseconds = 120000)
    {
        _withRetries = withRetries;
        _retryLimit = retryLimit;
        _httpClientTimeoutMilliseconds = httpClientTimeoutMilliseconds;
    }

    public ConnectionClient ConnectedTo(EndPoint endPoint)
    {
        return ConnectionClientFactory.Create(endPoint, _withRetries, _retryLimit, _httpClientTimeoutMilliseconds);
    }
}