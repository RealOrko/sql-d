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
    private static readonly string RootDirectoryPath = Path.GetDirectoryName(EntryAssemblyCodeBase.LocalPath);

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
            Log.Out.Info($"Content Root: {RootDirectoryPath}");
            Log.Out.Info($"Current directory: {Environment.CurrentDirectory}");

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