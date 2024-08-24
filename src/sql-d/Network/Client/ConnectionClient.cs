using SqlD.Exceptions;
using SqlD.Extensions;
using SqlD.Extensions.Discovery;
using SqlD.Logging;
using SqlD.Network.Client.Json;
using SqlD.Network.Server.Api.Db.Model;
using SqlD.Network.Server.Api.Kill.Model;

namespace SqlD.Network.Client
{
	public class ConnectionClient : IDisposable
	{
		private readonly IAsyncJsonService client;

		public EndPoint EndPoint { get; }
		public bool WithRetries { get; }

		internal ConnectionClient(EndPoint endPoint, bool withRetries, int retryLimit, int httpClientTimeoutMilliseconds)
		{
			this.EndPoint = endPoint;
			this.WithRetries = withRetries;

			if (withRetries)
			{
				client = new AsyncJsonServiceWithRetry(retryLimit, httpClientTimeoutMilliseconds);
			}
			else
			{
				client = new AsyncJsonService(httpClientTimeoutMilliseconds);
			}
		}

		public virtual bool Ping(EndPoint remoteEndPoint = null)
		{
			try
			{
				var response = client.GetAsync(UrlBuilder.GetPingUrl(remoteEndPoint ?? this.EndPoint)).GetAwaiter().GetResult();
				if (response.IsSuccessStatusCode)
					return true;
			}
			catch { }

			return false;
		}

		public virtual async Task<bool> PingAsync(EndPoint remoteEndPoint = null)
		{
			try
			{
				var response = await client.GetAsync(UrlBuilder.GetPingUrl(remoteEndPoint ?? this.EndPoint));
				if (response.IsSuccessStatusCode)
					return true;
			}
			catch { }

			return false;
		}

		public virtual List<T> Query<T>(string @where = null) where T : new()
		{
			var query = new Query()
			{
				Select = typeof(T).GetSelect(where),
				Columns = typeof(T).GetColumns(),
				Properties = typeof(T).GetPropertyInfoNames()
			};

			var result = PostQuery(query);
			return result.TransformTo<T>();
		}

		public virtual async Task<List<T>> QueryAsync<T>(string @where = null) where T : new()
		{
			var query = new Query()
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

		public virtual void Kill()
		{
			try
			{
				Log.Out.Info($"Connection Client sending kill to {EndPoint.ToUrl()}");
				var killUrl = UrlBuilder.GetKillUrl(EndPoint);
				client.PostAsync(killUrl, new KillRequest()
				{
					EndPoint = EndPoint
				}).GetAwaiter().GetResult();
			}
			catch (Exception err)
			{
				Log.Out.Info($"Connection Client kill result, failure here is expected, downgrading to info");
				Log.Out.Info($"{err.Message}");
				Log.Out.Info($"{err.StackTrace}");
			}
		}

		public virtual async Task KillAsync()
		{
			try
			{
				var killUrl = UrlBuilder.GetKillUrl(EndPoint);
				await client.PostAsync(killUrl, new KillRequest()
				{
					EndPoint = EndPoint
				});
			}
			catch (Exception err)
			{
			    Log.Out.Info($"Connection Client kill result, failure here is expected, downgrading to info");
			    Log.Out.Info($"{err.Message}");
			    Log.Out.Info($"{err.StackTrace}");
			}

        }

        public virtual TRes Get<TReq, TRes>(string resource, TReq message = default(TReq)) where TReq: class
		{
			var response = client.GetAsync<TRes>(EndPoint.ToUrl(resource), message).GetAwaiter().GetResult();
			return response;
		}

		public virtual async Task<TRes> GetAsync<TReq, TRes>(string resource, TReq message = default(TReq)) where TReq: class
		{
			var response = await client.GetAsync<TRes>(EndPoint.ToUrl(resource), message);
			return response;
		}

		public virtual TRes Post<TReq, TRes>(string resource, TReq message = default(TReq))
		{
			var response = client.PostAsync<TRes>(EndPoint.ToUrl(resource), message).GetAwaiter().GetResult();
			return response;
		}

		public virtual async Task<TRes> PostAsync<TReq, TRes>(string resource, TReq message = default(TReq))
		{
			var response = await client.PostAsync<TRes>(EndPoint.ToUrl(resource), message);
			return response;
		}

		public virtual TRes Put<TReq, TRes>(string resource, TReq message = default(TReq))
		{
			var response = client.PutAsync<TRes>(EndPoint.ToUrl(resource), message).GetAwaiter().GetResult();
			return response;
		}

		public virtual async Task<TRes> PutAsync<TReq, TRes>(string resource, TReq message = default(TReq))
		{
			var response = await client.PutAsync<TRes>(EndPoint.ToUrl(resource), message);
			return response;
		}

		public virtual TRes Delete<TRes>(string resource)
		{
			var response = client.DeleteAsync<TRes>(EndPoint.ToUrl(resource)).GetAwaiter().GetResult();
			return response;
		}

		public virtual async Task<TRes> DeleteAsync<TRes>(string resource)
		{
			var response = await client.DeleteAsync<TRes>(EndPoint.ToUrl(resource));
			return response;
		}

		public virtual DescribeResponse DescribeCommand(Describe describe)
		{
			var describeUrl = UrlBuilder.GetDescribeUrl(EndPoint);
			var response = client.PostAsync<DescribeResponse>(describeUrl, describe).GetAwaiter().GetResult();
			if (response.StatusCode != StatusCode.Ok)
				throw new ConnectionClientCommandException($"Describe failed. {response.Error}");
			return response;
		}

		public virtual async Task<DescribeResponse> DescribeCommandAsync(Describe describe)
		{
			var describeUrl = UrlBuilder.GetDescribeUrl(EndPoint);
			var response = await client.PostAsync<DescribeResponse>(describeUrl, describe);
			if (response.StatusCode != StatusCode.Ok)
				throw new ConnectionClientCommandException($"Describe failed. {response.Error}");
			return response;
		}

		public virtual CommandResponse PostCommand(Command command)
		{
			var commandUrl = UrlBuilder.GetCommandUrl(EndPoint);
			var response = client.PostAsync<CommandResponse>(commandUrl, command).GetAwaiter().GetResult();
			if (response.StatusCode != StatusCode.Ok)
				throw new ConnectionClientCommandException($"Command failed. {response.Error}");
			return response;
		}

		public virtual async Task<CommandResponse> PostCommandAsync(Command command)
		{
			var commandUrl = UrlBuilder.GetCommandUrl(EndPoint);
			var response = await client.PostAsync<CommandResponse>(commandUrl, command);
			if (response.StatusCode != StatusCode.Ok)
				throw new ConnectionClientCommandException($"Command failed. {response.Error}");
			return response;
		}

		public virtual CommandResponse PostScalar(Command command)
		{
			var commandUrl = UrlBuilder.GetScalarUrl(EndPoint);
			var response = client.PostAsync<CommandResponse>(commandUrl, command).GetAwaiter().GetResult();
			if (response.StatusCode != StatusCode.Ok)
				throw new ConnectionClientCommandException($"Command failed. {response.Error}");
			return response;
		}

		public virtual async Task<CommandResponse> PostScalarAsync(Command command)
		{
			var commandUrl = UrlBuilder.GetScalarUrl(EndPoint);
			var response = await client.PostAsync<CommandResponse>(commandUrl, command);
			if (response.StatusCode != StatusCode.Ok)
				throw new ConnectionClientCommandException($"Command failed. {response.Error}");
			return response;
		}

		public virtual QueryResponse PostQuery(Query command)
		{
			var commandUrl = UrlBuilder.GetQueryUrl(EndPoint);
			var response = client.PostAsync<QueryResponse>(commandUrl, command).GetAwaiter().GetResult();
			if (response.StatusCode != StatusCode.Ok)
				throw new ConnectionClientCommandException($"Query failed. {response.Error}");
			return response;
		}

		public virtual async Task<QueryResponse> PostQueryAsync(Query command)
		{
			var commandUrl = UrlBuilder.GetQueryUrl(EndPoint);
			var response = await client.PostAsync<QueryResponse>(commandUrl, command);
			if (response.StatusCode != StatusCode.Ok)
				throw new ConnectionClientCommandException($"Query failed. {response.Error}");
			return response;
		}

		public virtual void Dispose()
		{
			ConnectionClientFactory.Remove(this);
			client?.Dispose();
			Log.Out.Info($"Disposed client on {EndPoint.ToUrl()}");
		}
	}
}