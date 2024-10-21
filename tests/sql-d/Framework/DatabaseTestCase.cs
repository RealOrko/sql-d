using System.Threading.Tasks;
using NUnit.Framework;
using SqlD.Builders;
using SqlD.Configs.Model;
using SqlD.Extensions;
using SqlD.Tests.Framework.Models;

namespace SqlD.Tests.Framework;

public class DatabaseTestCase<T> : BaseTestCase where T : IAmATestModel, new()
{
    protected DbConnection Connection;
    protected T Instance;

    [SetUp]
    public virtual async Task SetUp()
    {
        Connection = new NewDbBuilder().ConnectedTo("sql-d.db", SqlDPragmaModel.Default).Connect();

        var createTable = typeof(AnyTableA).GetCreateTable();
        await Connection.ExecuteCommandAsync(createTable);

        createTable = typeof(AnyTableB).GetCreateTable();
        await Connection.ExecuteCommandAsync(createTable);

        Instance = await InsertAny<T>();
    }

    [TearDown]
    public virtual async Task TearDown()
    {
        var dropTable = typeof(AnyTableA).GetDropTable();
        await Connection.ExecuteCommandAsync(dropTable);

        dropTable = typeof(AnyTableB).GetDropTable();
        await Connection.ExecuteCommandAsync(dropTable);
        
        Connection.Dispose();
    }

    protected virtual async Task<T1> InsertAny<T1>() where T1 : IAmATestModel, new()
    {
        var instance = GenFu.GenFu.New<T1>();
        await Connection.InsertAsync(instance);
        return instance;
    }

    protected virtual async Task DeleteAny<T1>(T1 instance) where T1 : IAmATestModel
    {
        await Connection.DeleteAsync(instance);
    }
}