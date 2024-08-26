using System.Linq;
using NUnit.Framework;
using SqlD.Network.Diagnostics;
using SqlD.Tests.Framework;

namespace SqlD.Tests.Network.Diagnostics
{
	[TestFixture]
	public class EndPointRegistryTests : NetworkTestCase
	{
		[Test]
		public void ShouldBeAbleToRegisterEndPoint()
		{
			EndPointRegistry.GetOrAdd(MasterService.ToEndPoint());
			Assert.That(EndPointRegistry.Get().Any(), Is.True);
		}
	}
}