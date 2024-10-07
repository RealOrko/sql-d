using System.IO;
using System.Linq;
using System.Net;
using NUnit.Framework;
using SqlD.Builders;
using SqlD.Configs.Model;
using SqlD.Network.Client;
using SqlD.Network.Diagnostics;

namespace SqlD.Tests.Framework;

public class NetworkTestCase
{
    const string ConfigurationFileName = "appsettings.json";
    
    public SqlDConfiguration SqlDConfig { get; private set; }

    public SqlDServiceModel RegistryService => SqlDConfig.Services.First(serviceModel => serviceModel.Name == "sql-d-registry-1");

    public SqlDServiceModel Slave1Service => SqlDConfig.Services.First(serviceModel => serviceModel.Name == "sql-d-slave-1");

    public SqlDServiceModel Slave2Service => SqlDConfig.Services.First(serviceModel => serviceModel.Name == "sql-d-slave-2");

    public SqlDServiceModel Slave3Service => SqlDConfig.Services.First(serviceModel => serviceModel.Name == "sql-d-slave-3");

    public SqlDServiceModel MasterService => SqlDConfig.Services.First(serviceModel => serviceModel.Name == "sql-d-master-1");

    public ConnectionClient RegistryClient => new NewClientBuilder(withRetries: false).ConnectedTo(RegistryService.ToEndPoint());
    
    public ConnectionClient Slave1Client => new NewClientBuilder(withRetries: false).ConnectedTo(Slave1Service.ToEndPoint());
    
    public ConnectionClient Slave2Client => new NewClientBuilder(withRetries: false).ConnectedTo(Slave2Service.ToEndPoint());
    
    public ConnectionClient Slave3Client => new NewClientBuilder(withRetries: false).ConnectedTo(Slave3Service.ToEndPoint());
    
    public ConnectionClient MasterClient => new NewClientBuilder(withRetries: false).ConnectedTo(MasterService.ToEndPoint());

    [SetUp]
    public void SetUp()
    {
        if (File.Exists("sql-d-master-1.db")) File.Delete("sql-d-master-1.db");
        if (File.Exists("sql-d-slave-1.db")) File.Delete("sql-d-slave-1.db");
        if (File.Exists("sql-d-slave-2.db")) File.Delete("sql-d-slave-2.db");
        if (File.Exists("sql-d-slave-3.db")) File.Delete("sql-d-slave-3.db");
        if (File.Exists("sql-d-registry-1.db")) File.Delete("sql-d-registry-1.db");
        SqlDConfig = Interface.Start(GetType().Assembly, ConfigurationFileName);
        EndPointMonitor.WaitUntil(RegistryService.ToEndPoint(), EndPointIs.Up);
        EndPointMonitor.WaitUntil(Slave3Service.ToEndPoint(), EndPointIs.Up);
        EndPointMonitor.WaitUntil(Slave2Service.ToEndPoint(), EndPointIs.Up);
        EndPointMonitor.WaitUntil(Slave1Service.ToEndPoint(), EndPointIs.Up);
        EndPointMonitor.WaitUntil(MasterService.ToEndPoint(), EndPointIs.Up);
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