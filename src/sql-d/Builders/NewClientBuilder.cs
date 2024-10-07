using SqlD.Network;
using SqlD.Network.Client;

namespace SqlD.Builders
{
	internal class NewClientBuilder
	{
		private readonly int retryLimit;
		private readonly bool withRetries;
		private readonly int httpClientTimeoutMilliseconds;

		internal NewClientBuilder(bool withRetries, int retryLimit = 5, int httpClientTimeoutMilliseconds = 5000)
		{
			this.withRetries = withRetries;
			this.retryLimit = retryLimit;
			this.httpClientTimeoutMilliseconds = httpClientTimeoutMilliseconds;
		}

		public ConnectionClient ConnectedTo(EndPoint endPoint)
		{
			return ConnectionClientFactory.Get(endPoint, this.withRetries, this.retryLimit, this.httpClientTimeoutMilliseconds);
		}
	}
}