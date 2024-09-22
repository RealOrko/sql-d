using SqlD.Network.Diagnostics;

namespace SqlD.Network.Client.Json;

public class AsyncJsonServiceWithRetry : AsyncJsonService
{
    private readonly FastPollyPolicy _policy;

    public AsyncJsonServiceWithRetry(int retryLimit, int httpClientTimeoutMilliseconds) : base(httpClientTimeoutMilliseconds)
    {
        _policy = FastPolly
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryLimit,
                retry => TimeSpan.FromMilliseconds(retry * 250));
    }

    public override async Task<T> GetAsync<T>(string uri, object data = null)
    {
        return await _policy.ExecuteAsync(async () => await base.GetAsync<T>(uri, data));
    }

    public override async Task<HttpResponseMessage> GetAsync(string uri, object data = null)
    {
        return await _policy.ExecuteAsync(async () => await base.GetAsync(uri, data));
    }

    public override async Task<string> GetStringAsync(string uri, object data = null)
    {
        return await _policy.ExecuteAsync(async () => await base.GetStringAsync(uri, data));
    }

    public override async Task<T> PostAsync<T>(string uri, object data = null, bool dontSerialize = false)
    {
        return await _policy.ExecuteAsync(async () => await base.PostAsync<T>(uri, data, dontSerialize));
    }

    public override async Task<HttpResponseMessage> PostAsync(string uri, object data = null, bool dontSerialize = false)
    {
        return await _policy.ExecuteAsync(async () => await base.PostAsync(uri, data, dontSerialize));
    }

    public override async Task<T> PutAsync<T>(string uri, object data = null, bool dontSerialize = false)
    {
        return await _policy.ExecuteAsync(async () => await base.PutAsync<T>(uri, data, dontSerialize));
    }

    public override async Task<HttpResponseMessage> PutAsync(string uri, object data = null, bool dontSerialize = false)
    {
        return await _policy.ExecuteAsync(async () => await base.PutAsync(uri, data, dontSerialize));
    }

    public override async Task<T> DeleteAsync<T>(string uri)
    {
        return await _policy.ExecuteAsync(async () => await base.DeleteAsync<T>(uri));
    }

    public override async Task<HttpResponseMessage> DeleteAsync(string uri)
    {
        return await _policy.ExecuteAsync(async () => await base.DeleteAsync(uri));
    }
}