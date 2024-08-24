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

		public static IEqualityComparer<EndPoint> EndPointComparer { get; } = new EndPointEqualityComparer();

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

		protected bool Equals(EndPoint other)
		{
			return string.Equals(Host, other.Host) && Port == other.Port;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((EndPoint) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Host != null ? Host.GetHashCode() : 0) * 397) ^ Port;
			}
		}

		public static bool operator ==(EndPoint left, EndPoint right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(EndPoint left, EndPoint right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{Host}:{Port}";
		}

		private sealed class EndPointEqualityComparer : IEqualityComparer<EndPoint>
		{
			public bool Equals(EndPoint x, EndPoint y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return string.Equals(x.Host, y.Host) && x.Port == y.Port;
			}

			public int GetHashCode(EndPoint obj)
			{
				unchecked
				{
					return ((obj.Host != null ? obj.Host.GetHashCode() : 0) * 397) ^ obj.Port;
				}
			}
		}

	}
}