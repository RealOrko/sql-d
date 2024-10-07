using System.Linq;
using NUnit.Framework;
using SqlD.Tests.Framework.Network;
using NetworkRegistry = SqlD.Network.Server.Api.Registry.Registry;

namespace SqlD.Tests.Network.Server.Api.Registry
{
	[TestFixture]
	public class RegistryTests : NetworkTestCase
	{
		[Test]
		public void ShouldBeAbleToRegisterListenersIfRegistryIsUp()
		{
			NetworkRegistry.GetOrAdd(WellKnown.EndPoints.Free1);

			Assert.That(NetworkRegistry.Get().Any(x => x.Equals(WellKnown.EndPoints.Free1)), Is.True);

			NetworkRegistry.Register(WellKnown.Listeners.AlphaListener, "alpha");
			Assert.That(NetworkRegistry.List().Any(x => x.ToEndPoint().Equals(WellKnown.EndPoints.Alpha)), Is.True);
		}
	}
}