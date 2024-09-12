namespace SqlD.Network
{
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

		public static EndPoint Local(int port)
		{
			return new EndPoint("localhost", port);
		}

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

		public static bool operator ==(EndPoint left, EndPoint right)
		{
			return left.Host == right.Host && left.Port == right.Port;
		}

		public static bool operator !=(EndPoint left, EndPoint right)
		{
			return left.Host != right.Host && left.Port != right.Port;
		}

		public override string ToString()
		{
			return $"{Host}:{Port}";
		}
	}
}