using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SqlD.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
