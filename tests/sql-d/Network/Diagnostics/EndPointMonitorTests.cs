using NUnit.Framework;
using SqlD.Network;
using SqlD.Network.Diagnostics;
using SqlD.Tests.Framework;

namespace SqlD.Tests.Network.Diagnostics
{
	[TestFixture]
	public class EndPointMonitorTests : NetworkTestCase
	{
		[Test]
		public void ShouldBeAbleToDetectListenerUpDownEvents()
		{
			EndPointMonitor endPointMonitor = null;

			try
			{
				var masterIsUp = false;

				endPointMonitor = new EndPointMonitor(MasterService.ToEndPoint());

				endPointMonitor.OnUp += args => masterIsUp = true;
				endPointMonitor.OnDown += args => masterIsUp = false;

				endPointMonitor.WaitUntil(Constants.END_POINT_UP_WAIT_FOR_TIMEOUT, EndPointIs.Up);
				endPointMonitor.DoEvents();

				Assert.That(masterIsUp, Is.True);
			}
			finally
			{
				endPointMonitor?.Dispose();
			}
		}
	}
}