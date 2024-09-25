using System.Net;
using SqlD.Exceptions;
using SqlD.Extensions;
using SqlD.Extensions.Discovery;
using SqlD.Logging;
using SqlD.Network.Client.Json;
using SqlD.Network.Server.Api.Db.Model;
using SqlD.Network.Server.Api.Unregister.Model;

namespace SqlD.Network.Client;

public class ConnectionClient : IDisposable
{
    private readonly IAsyncJsonService _client;

    internal ConnectionClient(EndPoint endPoint, bool withRetries, int retryLimit, int httpClientTimeoutMilliseconds)
    {
        EndPoint = endPoint;
        WithRetries = withRetries;

        if (withRetries)
            _client = new AsyncJsonServiceWithRetry(retryLimit, httpClientTimeoutMilliseconds);
        else
            _client = new AsyncJsonService(httpClientTimeoutMilliseconds);
    }

    public EndPoint EndPoint { get; }
    public bool WithRetries { get; }

    public virtual void Dispose()
    {
        _client?.Dispose();
        Log.Out.Debug($"Disposed client on {EndPoint.ToUrl()}");
    }

    public virtual bool Ping(EndPoint remoteEndPoint = null)
    {
        try
        {
            var response = _client.GetAsync(UrlBuilder.GetPingUrl(remoteEndPoint ?? EndPoint)).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
                return true;
        }
        catch
        {
        }

        return false;
    }

    public virtual async Task<bool> PingAsync(EndPoint remoteEndPoint = null)
    {
        try
        {
            var response = await _client.GetAsync(UrlBuilder.GetPingUrl(remoteEndPoint ?? EndPoint));
            if (response.IsSuccessStatusCode)
                return true;
        }
        catch
        {
        }

        return false;
    }

    public virtual List<T> Query<T>(string where = null) where T : new()
    {
        var query = new Query
        {
            Select = typeof(T).GetSelect(where),
            Columns = typeof(T).GetColumns(),
            Properties = typeof(T).GetPropertyInfoNames()
        };

        var result = PostQuery(query);
        return result.TransformTo<T>();
    }

    public virtual async Task<List<T>> QueryAsync<T>(string where = null) where T : new()
    {
        var query = new Query
        {
            Select = typeof(T).GetSelect(where),
            Columns = typeof(T).GetColumns(),
            Properties = typeof(T).GetPropertyInfoNames()
        };

        var result = await PostQueryAsync(query);
        return result.TransformTo<T>();
    }

    public virtual void Insert<T>(T instance)
    {
        var command = new Command();
        command.AddInsert(instance);

        var response = PostScalar(command);
        if (response.ScalarResults.Any())
        {
            var idProperty = PropertyDiscovery.GetProperty("Id", typeof(T));
            idProperty.SetValue(instance, response.ScalarResults.First());
        }
    }

    public virtual async Task InsertAsync<T>(T instance)
    {
        var command = new Command();
        command.AddInsert(instance);

        var response = await PostScalarAsync(command);
        if (response.ScalarResults.Any())
        {
            var idProperty = PropertyDiscovery.GetProperty("Id", typeof(T));
            idProperty.SetValue(instance, response.ScalarResults.First());
        }
    }

    public virtual void InsertMany<T>(IEnumerable<T> instances, bool withIdentity = false)
    {
        var command = new Command();
        command.AddManyInserts(instances.Cast<object>(), withIdentity);

        var response = PostScalar(command);
        if (response.ScalarResults.Any())
        {
            var instanceList = instances.ToList();
            var idProperty = PropertyDiscovery.GetProperty("Id", typeof(T));
            for (var index = 0; index < instanceList.Count; index++)
            {
                var instance = instanceList[index];
                var identity = Convert.ToInt64(response.ScalarResults[index]);
                idProperty.SetValue(instance, identity);
            }
        }
    }

    public virtual async Task InsertManyAsync<T>(IEnumerable<T> instances, bool withIdentity = false)
    {
        var command = new Command();
        command.AddManyInserts(instances.Cast<object>(), withIdentity);

        var response = await PostScalarAsync(command);
        if (response.ScalarResults.Any())
        {
            var instanceList = instances.ToList();
            var idProperty = PropertyDiscovery.GetProperty("Id", typeof(T));
            for (var index = 0; index < instanceList.Count; index++)
            {
                var instance = instanceList[index];
                var identity = Convert.ToInt64(response.ScalarResults[index]);
                idProperty.SetValue(instance, identity);
            }
        }
    }

    public virtual void Update<T>(T instance)
    {
        var command = new Command();
        command.AddUpdate(instance);
        PostCommand(command);
    }

    public virtual async Task UpdateAsync<T>(T instance)
    {
        var command = new Command();
        command.AddUpdate(instance);
        await PostCommandAsync(command);
    }

    public virtual void UpdateMany<T>(IEnumerable<T> instances)
    {
        var command = new Command();
        command.AddManyUpdates(instances.Cast<object>());
        PostCommand(command);
    }

    public virtual async Task UpdateManyAsync<T>(IEnumerable<T> instances)
    {
        var command = new Command();
        command.AddManyUpdates(instances.Cast<object>());
        await PostCommandAsync(command);
    }

    public virtual void Delete<T>(T instance)
    {
        var command = new Command();
        command.AddDelete(instance);
        PostCommand(command);
    }

    public virtual async Task DeleteAsync<T>(T instance)
    {
        var command = new Command();
        command.AddDelete(instance);
        await PostCommandAsync(command);
    }

    public virtual void DeleteMany<T>(IEnumerable<T> instances)
    {
        var command = new Command();
        command.AddManyDeletes(instances.Cast<object>());
        PostCommand(command);
    }

    public virtual async Task DeleteManyAsync<T>(IEnumerable<T> instances)
    {
        var command = new Command();
        command.AddManyDeletes(instances.Cast<object>());
        await PostCommandAsync(command);
    }

    public virtual void CreateTable<T>()
    {
        var createTable = typeof(T).GetCreateTable();
        var command = new Command();
        command.Commands.Add(createTable);
        PostCommand(command);
    }

    public virtual async Task CreateTableAsync<T>()
    {
        var createTable = typeof(T).GetCreateTable();
        var command = new Command();
        command.Commands.Add(createTable);
        await PostCommandAsync(command);
    }

    public virtual void Unregister()
    {
        Log.Out.Info($"Connection Client sending unregister to {EndPoint.ToUrl()}");
        var unregisterUrl = UrlBuilder.GetUnregisterUrl(EndPoint);
        _client.PostAsync(unregisterUrl, new UnregisterRequest
        {
            EndPoint = EndPoint
        }).GetAwaiter().GetResult();
    }

    public virtual async Task UnregisterAsync()
    {
        var killUrl = UrlBuilder.GetUnregisterUrl(EndPoint);
        await _client.PostAsync(killUrl, new UnregisterRequest
        {
            EndPoint = EndPoint
        });
    }

    public virtual TRes Get<TReq, TRes>(string resource, TReq message = default) where TReq : class
    {
        var response = _client.GetAsync<TRes>(EndPoint.ToUrl(resource), message).GetAwaiter().GetResult();
        return response;
    }

    public virtual async Task<TRes> GetAsync<TReq, TRes>(string resource, TReq message = default) where TReq : class
    {
        var response = await _client.GetAsync<TRes>(EndPoint.ToUrl(resource), message);
        return response;
    }

    public virtual TRes Post<TReq, TRes>(string resource, TReq message = default)
    {
        var response = _client.PostAsync<TRes>(EndPoint.ToUrl(resource), message).GetAwaiter().GetResult();
        return response;
    }

    public virtual async Task<TRes> PostAsync<TReq, TRes>(string resource, TReq message = default)
    {
        var response = await _client.PostAsync<TRes>(EndPoint.ToUrl(resource), message);
        return response;
    }

    public virtual TRes Put<TReq, TRes>(string resource, TReq message = default)
    {
        var response = _client.PutAsync<TRes>(EndPoint.ToUrl(resource), message).GetAwaiter().GetResult();
        return response;
    }

    public virtual async Task<TRes> PutAsync<TReq, TRes>(string resource, TReq message = default)
    {
        var response = await _client.PutAsync<TRes>(EndPoint.ToUrl(resource), message);
        return response;
    }

    public virtual TRes Delete<TRes>(string resource)
    {
        var response = _client.DeleteAsync<TRes>(EndPoint.ToUrl(resource)).GetAwaiter().GetResult();
        return response;
    }

    public virtual async Task<TRes> DeleteAsync<TRes>(string resource)
    {
        var response = await _client.DeleteAsync<TRes>(EndPoint.ToUrl(resource));
        return response;
    }

    public virtual DescribeResponse DescribeCommand(Describe describe)
    {
        var describeUrl = UrlBuilder.GetDescribeUrl(EndPoint);
        var response = _client.PostAsync<DescribeResponse>(describeUrl, describe).GetAwaiter().GetResult();
        if (response.StatusCode != StatusCode.Ok)
            throw new ConnectionClientCommandException($"Describe failed. {response.Error}");
        return response;
    }

    public virtual async Task<DescribeResponse> DescribeCommandAsync(Describe describe)
    {
        var describeUrl = UrlBuilder.GetDescribeUrl(EndPoint);
        var response = await _client.PostAsync<DescribeResponse>(describeUrl, describe);
        if (response.StatusCode != StatusCode.Ok)
            throw new ConnectionClientCommandException($"Describe failed. {response.Error}");
        return response;
    }

    public virtual CommandResponse PostCommand(Command command)
    {
        var commandUrl = UrlBuilder.GetCommandUrl(EndPoint);
        var response = _client.PostAsync<CommandResponse>(commandUrl, command).GetAwaiter().GetResult();
        if (response.StatusCode != StatusCode.Ok)
            throw new ConnectionClientCommandException($"Command failed. {response.Error}");
        return response;
    }

    public virtual async Task<CommandResponse> PostCommandAsync(Command command)
    {
        var commandUrl = UrlBuilder.GetCommandUrl(EndPoint);
        var response = await _client.PostAsync<CommandResponse>(commandUrl, command);
        if (response.StatusCode != StatusCode.Ok)
            throw new ConnectionClientCommandException($"Command failed. {response.Error}");
        return response;
    }

    public virtual CommandResponse PostScalar(Command command)
    {
        var commandUrl = UrlBuilder.GetScalarUrl(EndPoint);
        var response = _client.PostAsync<CommandResponse>(commandUrl, command).GetAwaiter().GetResult();
        if (response.StatusCode != StatusCode.Ok)
            throw new ConnectionClientCommandException($"Command failed. {response.Error}");
        return response;
    }

    public virtual async Task<CommandResponse> PostScalarAsync(Command command)
    {
        var commandUrl = UrlBuilder.GetScalarUrl(EndPoint);
        var response = await _client.PostAsync<CommandResponse>(commandUrl, command);
        if (response.StatusCode != StatusCode.Ok)
            throw new ConnectionClientCommandException($"Command failed. {response.Error}");
        return response;
    }

    public virtual QueryResponse PostQuery(Query command)
    {
        var commandUrl = UrlBuilder.GetQueryUrl(EndPoint);
        var response = _client.PostAsync<QueryResponse>(commandUrl, command).GetAwaiter().GetResult();
        if (response.StatusCode != StatusCode.Ok)
            throw new ConnectionClientCommandException($"Query failed. {response.Error}");
        return response;
    }

    public virtual async Task<QueryResponse> PostQueryAsync(Query command)
    {
        var commandUrl = UrlBuilder.GetQueryUrl(EndPoint);
        var response = await _client.PostAsync<QueryResponse>(commandUrl, command);
        if (response.StatusCode != StatusCode.Ok)
            throw new ConnectionClientCommandException($"Query failed. {response.Error}");
        return response;
    }

    public async Task DownloadDatabaseTo(string databasePath)
    {
        Log.Out.Debug($"Downloading database from {EndPoint.ToUrl()} to {databasePath} ... ");
        
        var fileStreamUrl = UrlBuilder.GetFileStreamUrl(EndPoint);
        var response = await _client.GetAsync(fileStreamUrl);
        
        if (response.StatusCode != HttpStatusCode.OK)
            throw new ConnectionClientCommandException($"FileStream failed.");
        
        await using (var targetFileStream = File.Create(databasePath))
        {
            await response.Content.CopyToAsync(targetFileStream);
        }        
    }

    public async Task SynchroniseWith(EndPoint endPoint)
    {
        Log.Out.Info($"Synchronising database with {EndPoint.ToUrl()} ... ");
        
        var fileSyncUrl = UrlBuilder.GetFileSyncUrl(EndPoint);
        var response = await _client.PostAsync(fileSyncUrl, endPoint);
        
        if (response.StatusCode != HttpStatusCode.OK)
            throw new ConnectionClientCommandException($"FileSync failed.");
    }
    
    public async Task<string> SynchroniseHash()
    {
        var fileHashUrl = UrlBuilder.GetFileHashUrl(EndPoint);
        var response = await _client.GetAsync<string>(fileHashUrl);
        Log.Out.Debug($"Checking synchronisation hash for {EndPoint.ToUrl()} as '{response}' ");
        return response;
    }
}