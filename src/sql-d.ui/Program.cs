﻿using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SqlD.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
			if (!File.Exists("appsettings.json")) 
			{
				File.WriteAllText("appsettings.json", @"{
	""Logging"": {
		""IncludeScopes"": false,
		""LogLevel"": {
			""System"": ""Error"",
			""Default"": ""Error"",
			""Microsoft"": ""Error""
		}
	},
	""SqlD"": {
		""enabled"": true,
		""authority"": ""localhost"",
		""processmodel"": {
			""distributed"": false
		},
		""registries"": [
			{
				""host"": ""localhost"",
				""port"": 50095
			}
		],
		""services"": [
			{
				""name"": ""sql-d-registry-1"",
				""database"": ""sql-d-registry-1.db"",
				""host"": ""localhost"",
				""port"": 50095,
				""tags"": [""registry""],
				""pragma"": {
					""journalMode"": ""OFF"",
					""synchronous"": ""OFF"",
					""tempStore"": ""OFF"",
					""lockingMode"": ""OFF"",	
					""countChanges"": ""OFF"",
					""pageSize"": ""65536"",
					""cacheSize"": ""10000"", 
					""queryOnly"": ""OFF""
				}
			},
			{
				""name"": ""sql-d-slave-1"",
				""database"": ""sql-d-slave-1.db"",
				""host"": ""localhost"",
				""port"": 50101,
				""tags"": [""slave 1""],
				""pragma"": {
					""journalMode"": ""OFF"",
					""synchronous"": ""OFF"",
					""tempStore"": ""OFF"",
					""lockingMode"": ""OFF"",
					""countChanges"": ""OFF"",
					""pageSize"": ""65536"",
					""cacheSize"": ""10000"",
					""queryOnly"": ""OFF""
				}
			},
			{
				""name"": ""sql-d-slave-2"",
				""database"": ""sql-d-slave-2.db"",
				""host"": ""localhost"",
				""port"": 50102,
				""tags"": [""slave 2""],
				""pragma"": {
					""journalMode"": ""OFF"",
					""synchronous"": ""OFF"",
					""tempStore"": ""OFF"",
					""lockingMode"": ""OFF"",
					""countChanges"": ""OFF"",
					""pageSize"": ""65536"",
					""cacheSize"": ""10000"",
					""queryOnly"": ""OFF""
				}
			},
			{
				""name"": ""sql-d-slave-3"",
				""database"": ""sql-d-slave-3.db"",
				""host"": ""localhost"",
				""port"": 50103,
				""tags"": [""slave 3""],
				""pragma"": {
					""journalMode"": ""OFF"",
					""synchronous"": ""OFF"",
					""tempStore"": ""OFF"",
					""lockingMode"": ""OFF"",
					""countChanges"": ""OFF"",
					""pageSize"": ""65536"",
					""cacheSize"": ""10000"",
					""queryOnly"": ""OFF""
				}
			},
			{
				""name"": ""sql-d-master-1"",
				""database"": ""sql-d-master-1.db"",
				""host"": ""localhost"",
				""port"": 50100,
				""tags"": [""master""],
				""pragma"": {
					""journalMode"": ""OFF"",
					""synchronous"": ""OFF"",
					""tempStore"": ""OFF"",
					""lockingMode"": ""OFF"",
					""countChanges"": ""OFF"",
					""pageSize"": ""65536"",
					""cacheSize"": ""10000"",
					""queryOnly"": ""OFF""
				},
				""forwardingTo"": [
					{
						""host"": ""localhost"",
						""port"": 50101
					},
					{
						""host"": ""localhost"",
						""port"": 50102
					},
					{
						""host"": ""localhost"",
						""port"": 50103
					}
				]
			}
		]
	}
}");
			}

	        var config = typeof(Program).Assembly.SqlDGo();
	        try
	        {
		        BuildWebHost(args)?.Run();
	        }
	        finally
	        {
				SqlDStart.SqlDStop(config);
	        }
		}

		public static IWebHost BuildWebHost(string[] args)
	    {
		    try
		    {
			    return WebHost.CreateDefaultBuilder(args)
					.UseKestrel(opts => {
						opts.ListenAnyIP(5000);
					})
				    .UseStartup<Startup>()
				    //.UseUrls("http://+:5000")
				    .Build();
		    }
		    catch (Exception err)
		    {
				Console.WriteLine(err);
				Console.WriteLine("Press any key to continue....");
			    return null;
		    }
	    }
    }
}
