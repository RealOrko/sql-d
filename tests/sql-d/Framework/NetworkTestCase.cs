using System.IO;
using System.Linq;
using NUnit.Framework;
using SqlD.Builders;
using SqlD.Configs.Model;
using SqlD.Network.Client;
using SqlD.Network.Diagnostics;

namespace SqlD.Tests.Framework;

public class NetworkTestCase
{
    private const string ConfigurationFileName = "appsettings.json";

    static NetworkTestCase()
    {
        Interface.Setup(typeof(NetworkTestCase).Assembly, ConfigurationFileName);
    }

    public SqlDConfiguration SqlDConfig => SqlD.Configs.Configuration.Instance;

    public SqlDServiceModel RegistryService => SqlDConfig.Services.First(serviceModel => serviceModel.Name == "sql-d-registry-1");

    public SqlDServiceModel Slave1Service => SqlDConfig.Services.First(serviceModel => serviceModel.Name == "sql-d-slave-1");

    public SqlDServiceModel Slave2Service => SqlDConfig.Services.First(serviceModel => serviceModel.Name == "sql-d-slave-2");

    public SqlDServiceModel Slave3Service => SqlDConfig.Services.First(serviceModel => serviceModel.Name == "sql-d-slave-3");

    public SqlDServiceModel MasterService => SqlDConfig.Services.First(serviceModel => serviceModel.Name == "sql-d-master-1");

    public ConnectionClient RegistryClient => new NewClientBuilder(false).ConnectedTo(RegistryService);

    public ConnectionClient Slave1Client => new NewClientBuilder(false).ConnectedTo(Slave1Service);

    public ConnectionClient Slave2Client => new NewClientBuilder(false).ConnectedTo(Slave2Service);

    public ConnectionClient Slave3Client => new NewClientBuilder(false).ConnectedTo(Slave3Service);

    public ConnectionClient MasterClient => new NewClientBuilder(false).ConnectedTo(MasterService);

    [SetUp]
    public void SetUp()
    {
        if (File.Exists("sql-d-master-1.db")) File.Delete("sql-d-master-1.db");
        if (File.Exists("sql-d-slave-1.db")) File.Delete("sql-d-slave-1.db");
        if (File.Exists("sql-d-slave-2.db")) File.Delete("sql-d-slave-2.db");
        if (File.Exists("sql-d-slave-3.db")) File.Delete("sql-d-slave-3.db");
        if (File.Exists("sql-d-registry-1.db")) File.Delete("sql-d-registry-1.db");
        Interface.Start();
        EndPointMonitor.WaitUntil(RegistryService, EndPointIs.Up);
        EndPointMonitor.WaitUntil(Slave3Service, EndPointIs.Up);
        EndPointMonitor.WaitUntil(Slave2Service, EndPointIs.Up);
        EndPointMonitor.WaitUntil(Slave1Service, EndPointIs.Up);
        EndPointMonitor.WaitUntil(MasterService, EndPointIs.Up);
    }

    [TearDown]
    public void TearDown()
    {
        Interface.Stop();
        File.Delete("sql-d-master-1.db");
        File.Delete("sql-d-slave-1.db");
        File.Delete("sql-d-slave-2.db");
        File.Delete("sql-d-slave-3.db");
        File.Delete("sql-d-registry-1.db");
    }
}