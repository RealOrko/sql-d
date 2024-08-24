namespace SqlD.Network.Client.Json
{
    public interface IAsyncJsonService : IDisposable
    {
        Task<T> GetAsync<T>(string uri, object data = null);
        Task<HttpResponseMessage> GetAsync(string uri, object data = null);
        Task<string> GetStringAsync(string uri, object data = null);
        Task<T> PostAsync<T>(string uri, object data = null, bool dontSerialize = false);
        Task<HttpResponseMessage> PostAsync(string uri, object data = null, bool dontSerialize = false);
        Task<T> PutAsync<T>(string uri, object data = null, bool dontSerialize = false);
        Task<HttpResponseMessage> PutAsync(string uri, object data = null, bool dontSerialize = false);
        Task<T> DeleteAsync<T>(string uri);
        Task<HttpResponseMessage> DeleteAsync(string uri);
        void SetHeader(string header, string value);
        void ClearHeader(string header);
        void ClearHeaders();
        void EnableOnlySuccessOnlyMode(bool successOnly = true);
    }
}