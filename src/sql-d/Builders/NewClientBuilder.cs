using SqlD.Network;
using SqlD.Network.Client;

namespace SqlD.Builders
{
	public class NewClientBuilder
	{
		private readonly int retryLimit;
		private readonly bool withRetries;
		private readonly int httpClientTimeoutMilliseconds;

		internal NewClientBuilder(bool withRetries, int retryLimit, int httpClientTimeoutMilliseconds)
		{
			this.withRetries = withRetries;
			this.retryLimit = retryLimit;
			this.httpClientTimeoutMilliseconds = httpClientTimeoutMilliseconds;
		}

		public ConnectionClient ConnectedTo(EndPoint endPoint)
		{
			return ConnectionClientFactory.Get(endPoint, this.withRetries, this.retryLimit, this.httpClientTimeoutMilliseconds);
		}

		public async Task<bool> PingAsync(EndPoint endPoint)
		{
			var client = ConnectionClientFactory.Get(endPoint, this.withRetries, this.retryLimit, this.httpClientTimeoutMilliseconds);
			return await client.PingAsync();
		}
	}
}