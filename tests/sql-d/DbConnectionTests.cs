using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SqlD.Extensions;
using SqlD.Tests.Framework;
using SqlD.Tests.Framework.Models;

namespace SqlD.Tests;

[TestFixture]
public class DbConnectionTests : DatabaseTestCase<AnyTableA>
{
    [Test]
    public async Task ShouldBeAbleToQuery()
    {
        var anyQueryResult = await Connection.QueryAsync<AnyTableA>($"WHERE Any_String = '{Instance.AnyString}' AND Any_Integer = {Instance.AnyInteger}");
        Assert.That(anyQueryResult.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task ShouldBeAbleToInsert()
    {
        var instance = InsertAny<AnyTableB>();
        var anyQueryResult = await Connection.QueryAsync<AnyTableB>($"WHERE Id = {instance.Id}");
        Assert.That(anyQueryResult.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task ShouldBeAbleToUpdate()
    {
        Instance.AnyString = "Side effect!";
        await Connection.UpdateAsync(Instance);

        var instance = (await Connection.QueryAsync<AnyTableA>($"WHERE Id = {Instance.Id}")).FirstOrDefault();
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.AnyString, Is.EqualTo("Side effect!"));
    }

    [Test]
    public async Task ShouldBeAbleToDelete()
    {
        await DeleteAny(Instance);
        var items = await Connection.QueryAsync<AnyTableA>();
        Assert.That(items.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldBeAbleToCreateAndDrop()
    {
        var createTable = typeof(AnyTableB).GetCreateTable();
        await Connection.ExecuteCommandAsync(createTable);

        var dropTable = typeof(AnyTableB).GetDropTable();
        await Connection.ExecuteCommandAsync(dropTable);
    }

    #if Linux
    // This does not work on windows because of file locks
    [Test]
    public async Task ShouldBeAbleToLoadFileStream()
    {
        var bytes = new List<byte>();
        using (var databaseStream = Connection.GetDatabaseFileStream())
        {
            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            while ((bytesRead = databaseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                bytes.AddRange(buffer.Take(bytesRead));
            }
        }
        Assert.That(bytes.Count, Is.GreaterThan(0));
    }
    #endif
}