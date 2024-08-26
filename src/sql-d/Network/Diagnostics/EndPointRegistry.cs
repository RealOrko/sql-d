using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace SqlD.Network.Diagnostics
{
	public class EndPointRegistry
	{
		internal static readonly ConcurrentDictionary<EndPoint, EndPoint> EndPoints = new();

		public static ImmutableArray<EndPoint> Get()
		{
			return EndPoints.Values.Distinct(EndPoint.EndPointComparer).ToImmutableArray();
		}

		public static void GetOrAdd(EndPoint endPoint)
		{
			EndPoints.GetOrAdd(endPoint, endPoint);
		}

		public static void Reset()
		{
			EndPoints.Clear();
		}
	}
}