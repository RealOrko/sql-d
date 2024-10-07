using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace SqlD.Network.Diagnostics
{
	public class EndPointRegistry
	{
		private static readonly ConcurrentDictionary<EndPoint, EndPoint> EndPoints = new ConcurrentDictionary<EndPoint, EndPoint>();

		public static ImmutableArray<EndPoint> Get()
		{
			return EndPoints.Values.Distinct(EndPoint.EndPointComparer).ToImmutableArray();
		}

		public static void GetOrAdd(EndPoint endPoint)
		{
			EndPoints.GetOrAdd(endPoint, endPoint);
		}

		public static void TryRemove(EndPoint endPoint)
		{
			EndPoints.TryRemove(endPoint, out var e);
		}
	}
}