using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SqlD.Logging;

namespace SqlD.UI
{
    public class Program
    {
        private static readonly Uri EntryAssemblyCodeBase = new Uri(Assembly.GetEntryAssembly().Location);
        private static readonly string RootDirectoryPath = Path.GetDirectoryName(EntryAssemblyCodeBase.LocalPath);

        public static void Main(string[] args)
        {
	        var config = typeof(Program).Assembly.SqlDGo("appsettings.json");
	        try
	        {
		        BuildWebHost(args)?.Build().Run();
	        }
	        finally
	        {
				SqlDStart.SqlDStop();
	        }
		}

		public static IHostBuilder BuildWebHost(string[] args)
	    {
		    try
		    {
                Log.Out.Info($"Entry assembly: {EntryAssemblyCodeBase}");
                Log.Out.Info($"Content Root: {RootDirectoryPath}");
                Log.Out.Info($"Current directory: {Environment.CurrentDirectory}");

                return Host.CreateDefaultBuilder(args)
	                .ConfigureWebHostDefaults(builder => { builder.UseStartup<Startup>(); });
		    }
		    catch (Exception err)
		    {
				Log.Out.Error(err.ToString());
				Log.Out.Info("Press any key to continue....");
			    return null;
		    }
	    }
    }
}
