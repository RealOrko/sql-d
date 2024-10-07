using System.Threading.Tasks;
using NUnit.Framework;
using SqlD.Configs.Model;
using SqlD.Extensions;
using SqlD.Tests.Framework.Models;
using SqlD.Tests.Framework.Network;

namespace SqlD.Tests.Network.Server
{
	[TestFixture]
	public class ConnectionListenerTests : NetworkTestCase
	{
		[Test]
		public async Task ShouldBeAbleToInsertUpdateAndDelete()
		{
			await WellKnown.Clients.Free1Client.CreateTableAsync<AnyTableB>();

			const int maxItems = 25;

			var instances = GenFu.GenFu.ListOf<AnyTableB>(maxItems);

			await WellKnown.Clients.Free1Client.InsertManyAsync(instances);
			await AssertTargetHasCountMoreThanOrEqualTo(maxItems);

			await WellKnown.Clients.Free1Client.UpdateManyAsync(instances);
			await AssertTargetHasCountMoreThanOrEqualTo(maxItems);

			await WellKnown.Clients.Free1Client.DeleteManyAsync(instances);
			await AssertTargetHasCountMoreThanOrEqualTo(0);
		}

		private static async Task AssertTargetHasCountMoreThanOrEqualTo(int count = 0)
		{
			var countSql = typeof(AnyTableB).GetCount();
			var targetConnection = Interface.NewDb().ConnectedTo(WellKnown.Listeners.Free1Listener.DatabaseName, SqlDPragmaModel.Default);
			var targetResults = await targetConnection.ExecuteScalarAsync<long>(countSql);
			Assert.That(targetResults, Is.GreaterThanOrEqualTo(count));
		}
	}
}