using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SqlD.Logging;

namespace SqlD.UI;

public class Program
{
    private static readonly Uri EntryAssemblyCodeBase = new(Assembly.GetEntryAssembly().Location);

    public static void Main(string[] args)
    {
        Interface.Setup(typeof(Program).Assembly, "appsettings.json");
        Interface.Start();
        try
        {
            BuildWebHost(args)?.Build().Run();
        }
        finally
        {
            Interface.Stop();
        }
    }

    public static IHostBuilder BuildWebHost(string[] args)
    {
        try
        {
            Log.Out.Info($"Entry assembly: {EntryAssemblyCodeBase}");
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => { builder.UseStartup<Startup>(); });
        }
        catch (Exception err)
        {
            Log.Out.Error(err.ToString());
            return null;
        }
    }
}