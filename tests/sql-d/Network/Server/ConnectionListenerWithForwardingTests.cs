using System.Threading.Tasks;
using NUnit.Framework;
using SqlD.Tests.Framework;
using SqlD.Tests.Framework.Models;

namespace SqlD.Tests.Network.Server
{
	[TestFixture]
	public class ConnectionListenerWithForwardingTests : NetworkTestCase
	{
		[Test]
		public async Task ShouldBeAbleToValidateSingleInsertUpdateAndDeleteWithAllForwards()
		{
			await MasterClient.CreateTableAsync<AnyTableB>();

			var instances = GenFu.GenFu.New<AnyTableB>();

			await MasterClient.InsertAsync(instances);
			await MasterClient.UpdateAsync(instances);

			var results = await Slave1Client.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(1));

			results = await Slave2Client.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(1));

			results = await Slave3Client.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(1));

			await MasterClient.DeleteAsync(instances);

			results = await Slave1Client.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(0));
			
			results = await Slave2Client.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(0));

			results = await Slave3Client.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(0));
		}

		[Test]
		public async Task ShouldBeAbleToValidateMultipleInsertUpdateAndDeleteWithAllForwards()
		{
			await MasterClient.CreateTableAsync<AnyTableB>();

			var instances = GenFu.GenFu.ListOf<AnyTableB>(25);

			await MasterClient.InsertManyAsync(instances);
			await MasterClient.UpdateManyAsync(instances);

			var results = await Slave1Client.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(25));

			results = await Slave2Client.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(25));

			results = await Slave3Client.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(25));

			await MasterClient.DeleteManyAsync(instances);

			results = await Slave1Client.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(0));
			
			results = await Slave2Client.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(0));

			results = await Slave3Client.QueryAsync<AnyTableB>();
			Assert.That(results.Count, Is.EqualTo(0));
		}
	}
}