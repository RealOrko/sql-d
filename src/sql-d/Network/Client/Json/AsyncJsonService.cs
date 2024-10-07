using System.Collections.Concurrent;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlD.Extensions.System;
using SqlD.Extensions.System.Net;
using SqlD.Logging;

namespace SqlD.Network.Client.Json
{
	public class AsyncJsonService : IAsyncJsonService
    {
        private static readonly ConcurrentDictionary<string, string> Headers;
        private readonly HttpClient client;
        private readonly JsonSerializerSettings settings;
        private bool successOnly;

        static AsyncJsonService()
        {
            Headers = new ConcurrentDictionary<string, string>();
        }

        public AsyncJsonService(int httpClientTimeoutInMilliseconds)
        {
            client = new HttpClient();
			client.Timeout = TimeSpan.FromMilliseconds(httpClientTimeoutInMilliseconds); 
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        }

        public virtual async Task<T> GetAsync<T>(string uri, object data = null)
        {
            var result = await GetAsync(uri, data);
            if (successOnly)
                result.EnsureSuccessStatusCode();
            return await DeserialiseResponse<T>(result);
        }

        public virtual async Task<HttpResponseMessage> GetAsync(string uri, object data = null)
        {
            var fullUri = uri + data?.ToQueryString();
            var requestMessage = CreateRequest(HttpMethod.Get, fullUri);
            return await client.SendAsync(requestMessage);
        }

        public virtual async Task<string> GetStringAsync(string uri, object data = null)
        {
            var fullUri = uri + data?.ToQueryString();
            var requestMessage = CreateRequest(HttpMethod.Get, fullUri);
            var result = await client.SendAsync(requestMessage);
            if (successOnly)
                result.EnsureSuccessStatusCode();

            return await result.Content.ReadAsStringAsync();
        }

        public virtual async Task<T> PostAsync<T>(string uri, object data = null, bool dontSerialize = false)
        {
            var result = await PostAsync(uri, data, dontSerialize);
            if (successOnly)
                result.EnsureSuccessStatusCode();
            return await DeserialiseResponse<T>(result);
        }

        public virtual async Task<HttpResponseMessage> PostAsync(string uri, object data = null, bool dontSerialize = false)
        {
            var request = CreateRequest(HttpMethod.Post, uri);
            if (dontSerialize)
            {
                request.Content = new StringContent(data.SafeToString(), Encoding.UTF8, "application/json");
            }
            else
            {
                if (data != null)
                    request.Content = SerializeRequest(data);
            }
            return await client.SendAsync(request);
        }

        public virtual async Task<T> PutAsync<T>(string uri, object data = null, bool dontSerialize = false)
        {
            var result = await PutAsync(uri, data, dontSerialize);
            if (successOnly)
                result.EnsureSuccessStatusCode();
            return await DeserialiseResponse<T>(result);
        }

        public virtual async Task<HttpResponseMessage> PutAsync(string uri, object data = null, bool dontSerialize = false)
        {
            var request = CreateRequest(HttpMethod.Put, uri);
            if (dontSerialize)
            {
                request.Content = new StringContent(data.SafeToString(), Encoding.UTF8, "application/json");
            }
            else
            {
                if (data != null)
                    request.Content = SerializeRequest(data);
            }
            return await client.SendAsync(request);
        }

        public virtual async Task<T> DeleteAsync<T>(string uri)
        {
            var result = await DeleteAsync(uri);
            if (successOnly)
                result.EnsureSuccessStatusCode();
            return await DeserialiseResponse<T>(result);
        }

        public virtual async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            var request = CreateRequest(HttpMethod.Delete, uri);
            request.Content = SerializeRequest();
            return await client.SendAsync(request);
        }

        public virtual void SetHeader(string header, string value)
        {
            Headers[header] = value;
        }

        public virtual void ClearHeader(string header)
        {
            Headers[header] = null;
        }

        public virtual void ClearHeaders()
        {
            Headers.Clear();
        }

        public virtual void EnableOnlySuccessOnlyMode(bool successOnly = true)
        {
            this.successOnly = successOnly;
        }

        public virtual void Dispose()
        {
            client.Dispose();
        }

        private HttpContent SerializeRequest(object data = null)
        {
            StringContent request;
            if (data == null)
                request = new StringContent(string.Empty);
            else
                request = new StringContent(JsonConvert.SerializeObject(data, settings));
            request.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return request;
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string uri)
        {
            var requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            foreach (var headerValue in Headers.Where(xy => !string.IsNullOrEmpty(xy.Value)))
                requestMessage.Headers.Add(headerValue.Key, headerValue.Value);
            return requestMessage;
        }

        private async Task<T> DeserialiseResponse<T>(HttpResponseMessage result)
        {
            var isCompressed = false;
            if (result.Content.Headers.Contains("Content-Encoding"))
            {
                isCompressed = result.Content.Headers.GetValues("Content-Encoding").FirstOrDefault() == "gzip";
            }
            else
            {
                Log.Out.Warn("The response is not compressed using Content-Type=gzip!");
            }
            
            await using var responseStream = await result.Content.ReadAsStreamAsync();
            await using var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(isCompressed ? decompressedStream : responseStream);
            var payload = await streamReader.ReadToEndAsync();

            if (!result.IsSuccessStatusCode)
            {
                Log.Out.Error($"Error with response: {payload}");
            }

            return JsonConvert.DeserializeObject<T>(payload, settings);
        }
    }
}