# SqlD Help - SqlD

<div align="right">
	<a href="https://github.com/RealOrko/sql-d/blob/master/docs/_.md#sqld-help---contents">[Back to Contents]</a>
</div>

  * [NuGet](#nuget)
  * [Swagger](#swagger)
  * [Clients](#clients)

## NuGet

You can install SqlD from NuGet and reference it in a dotnet project. 

### Package Manager
```
PM> Install-Package sql-d
```

### .NET CLI
```
> dotnet add package sql-d
```

### CSharp Example

Here is an example creating a new SqlD connection listener.

```csharp
using System;
using SqlD;
using SqlD.Configuration.Model;
using SqlD.Network;

namespace New_SqlD_Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = SqlDStart.NewListener()
                .Hosting(
                    startAssembly: typeof(Program).Assembly, 
                    name: "NewService1", 
                    dbConnectionString: "NewService1.db", 
                    pragma: SqlDPragmaModel.Default, 
                    localEndPoint: EndPoint.FromUri("http://localhost:3020"));

            Console.ReadLine();
        }
    }
}
```

Here is an example of how you can connect to the listener using the connection client.

```csharp
using System;
using SqlD;
using SqlD.Network;

namespace New_SqlD_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = SqlDStart.NewClient().ConnectedTo(EndPoint.FromUri("http://localhost:3020"));
            if (client.Ping())
            {
                Console.WriteLine("Listener is up!");
            }

            Console.ReadLine();
        }
    }
}
```

 *See Also*:

  - [Clients](#clients)

## Swagger

<div align="right">
	<a href="#sqld-help---sqld">[Back to Top]</a>
</div>
<br/>

You have swagger support everywhere. Merely type `/swagger` at the end of any base endpoint url. 

Examples based on the [default configuration](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#defaults) for SqlD.UI:

 - `sqld.ui`: http://localhost:5000/swagger
 - `registry`: http://localhost:50095/swagger
 - `master`: http://localhost:50100/swagger 
 - `slave 1`: http://localhost:50101/swagger
 - `slave 2`: http://localhost:50102/swagger
 - `slave 3`: http://localhost:50103/swagger

Example:

![Swagger - UI](https://github.com/RealOrko/sql-d/blob/master/docs/images/sqld/swagger.png)

 *See Also*:

  - [Configuration](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md)

## Clients

<div align="right">
	<a href="#sqld-help---sqld">[Back to Top]</a>
</div>
<br/>

There is a really cool tool called NSwagStudio which can be downloaded from [here](https://github.com/RSuter/NSwag/wiki/NSwagStudio).

An NSwagStudio example solution that produces TypeScript and CSharp can be found [here](https://github.com/RealOrko/sql-d/blob/master/src/clients/sql-d.clients.nswag).

Some unofficial pre-generated clients can be found below. These are not supported here, please raise issues on https://github.com/RSuter/NSwag.

 - [Example CSharp Client](https://github.com/RealOrko/sql-d/blob/master/src/clients/csharp/Client.cs)
 - [Example TypeScript Client](https://github.com/RealOrko/sql-d/blob/master/src/clients/typescript/Client.ts)

 *See Also*:

  - [Configuration](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md)

