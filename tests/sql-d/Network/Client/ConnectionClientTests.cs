using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using SqlD.Tests.Framework;
using SqlD.Tests.Framework.Models;

namespace SqlD.Tests.Network.Client;

[TestFixture]
public class ConnectionClientTests : NetworkTestCase
{
    [Test]
    public async Task ShouldBeAbleToInsertUpdateAndDeleteWithSingle()
    {
        await MasterClient.CreateTableAsync<AnyTableB>();

        var instances = GenFu.GenFu.New<AnyTableB>();

        await MasterClient.InsertAsync(instances);
        await MasterClient.UpdateAsync(instances);

        var results = await MasterClient.QueryAsync<AnyTableB>();
        Assert.That(results.Count, Is.EqualTo(1));

        await MasterClient.DeleteAsync(instances);

        results = await MasterClient.QueryAsync<AnyTableB>();
        Assert.That(results.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldBeAbleToInsertUpdateAndDeleteWithMany()
    {
        await MasterClient.CreateTableAsync<AnyTableB>();

        var instances = GenFu.GenFu.ListOf<AnyTableB>(25);

        await MasterClient.InsertManyAsync(instances);
        await MasterClient.UpdateManyAsync(instances);

        var results = await MasterClient.QueryAsync<AnyTableB>();
        Assert.That(results.Count, Is.EqualTo(25));

        await MasterClient.DeleteManyAsync(instances);

        results = await MasterClient.QueryAsync<AnyTableB>();
        Assert.That(results.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldBeAbleToDownloadDatabaseFileFromClient()
    {
        await MasterClient.DownloadDatabaseTo("output.db");
        
        Assert.That(File.Exists("output.db"), Is.True);
    }
}