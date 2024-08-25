using System.Threading.Tasks;
using NUnit.Framework;
using SqlD.Configs.Model;
using SqlD.Extensions;
using SqlD.Tests.Framework.Models;
using SqlD.Tests.Framework.Network;

namespace SqlD.Tests.Network.Server
{
	[TestFixture]
	public class ConnectionListenerWithForwardingTests : NetworkTestCase
	{
		[Test]
		public async Task ShouldBeAbleToInsertUpdateAndDelete()
		{
			await WellKnown.Clients.Free1Client.CreateTableAsync<AnyTableB>();

			const int maxItems = 25;

			var instances = GenFu.GenFu.ListOf<AnyTableB>(maxItems);

			await WellKnown.Clients.Free1Client.InsertManyAsync(instances);
			AssertSourceHasCountMoreThanOrEqualTo(maxItems);
			AssertTargetHasCountMoreThanOrEqualTo(maxItems);

			await WellKnown.Clients.Free1Client.UpdateManyAsync(instances);
			AssertSourceHasCountMoreThanOrEqualTo(maxItems);
			AssertTargetHasCountMoreThanOrEqualTo(maxItems);

			await WellKnown.Clients.Free1Client.DeleteManyAsync(instances);
			AssertSourceHasCountMoreThanOrEqualTo(0);
			AssertTargetHasCountMoreThanOrEqualTo(0);
		}

		private void AssertTargetHasCountMoreThanOrEqualTo(int count = 0)
		{
			var countSql = typeof(AnyTableB).GetCount();
			var targetConnection = Interface.NewDb().ConnectedTo(WellKnown.Listeners.Free1Listener.DatabaseName, SqlDPragmaModel.Default);
			var targetResults = targetConnection.ExecuteScalar<long>(countSql);
			Assert.That(targetResults, Is.GreaterThanOrEqualTo(count));
		}

		private void AssertSourceHasCountMoreThanOrEqualTo(int count = 0)
		{
			var countSql = typeof(AnyTableB).GetCount();
			var sourceConnection = Interface.NewDb().ConnectedTo(WellKnown.Listeners.Free2Listener.DatabaseName, SqlDPragmaModel.Default);
			var sourceResultsCount = sourceConnection.ExecuteScalar<long>(countSql);
			Assert.That(sourceResultsCount, Is.GreaterThanOrEqualTo(count));
		}
	}
}