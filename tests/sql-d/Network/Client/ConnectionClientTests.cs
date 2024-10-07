using System.Threading.Tasks;
using NUnit.Framework;
using SqlD.Tests.Framework.Models;
using SqlD.Tests.Framework.Network;

namespace SqlD.Tests.Network.Client
{
	[TestFixture]
	public class ConnectionClientTests : NetworkTestCase
	{
		[Test]
		public async Task ShouldBeAbleToInsertUpdateAndDeleteWithSingle()
		{
			await WellKnown.Clients.AlphaClient.CreateTableAsync<AnyTableB>();

			var instances = GenFu.GenFu.New<AnyTableB>();

			await WellKnown.Clients.AlphaClient.InsertAsync(instances);
			await WellKnown.Clients.AlphaClient.UpdateAsync(instances);

			var results = await WellKnown.Clients.AlphaClient.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.GreaterThan(0));

			await WellKnown.Clients.AlphaClient.DeleteAsync(instances);

			results = await WellKnown.Clients.AlphaClient.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(0));
		}

		[Test]
		public async Task ShouldBeAbleToInsertUpdateAndDeleteWithMany()
		{
			await WellKnown.Clients.AlphaClient.CreateTableAsync<AnyTableB>();

			var instances = GenFu.GenFu.ListOf<AnyTableB>(25);

			await WellKnown.Clients.AlphaClient.InsertManyAsync(instances);
			await WellKnown.Clients.AlphaClient.UpdateManyAsync(instances);

			var results = await WellKnown.Clients.AlphaClient.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.GreaterThan(0));

			await WellKnown.Clients.AlphaClient.DeleteManyAsync(instances);

			results = await WellKnown.Clients.AlphaClient.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(0));
		}
	}
}