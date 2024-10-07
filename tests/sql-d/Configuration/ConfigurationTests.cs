using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SqlD.Network.Diagnostics;
using SqlD.Tests.Framework.Network;

namespace SqlD.Tests.Configuration
{
	[TestFixture]
	public class ConfigurationTests : NetworkTestCase
	{
		[Test]
		public void ShouldBeAbleToStartASqlDConfiguration()
		{
			var sqlD = GetType().Assembly.SqlDGo(@"./Configuration/appsettings.tests.json");
			ClassicAssert.NotNull(sqlD);

			var registry = sqlD.Services.First(x => x.Name.Equals("sql-d-registry-1"));
			ClassicAssert.Contains("registry", registry.Tags);

            EndPointMonitor.WaitUntil(registry.ToEndPoint(), EndPointIs.Up);

			var master = sqlD.Services.First(x => x.Name.Equals("sql-d-master-1"));
			ClassicAssert.Contains("master", master.Tags);

            EndPointMonitor.WaitUntil(master.ToEndPoint(), EndPointIs.Up);
        }
    }
}