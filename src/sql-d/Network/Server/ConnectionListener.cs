using System.Net;
using System.Reflection;
using SqlD.Logging;

namespace SqlD.Network.Server
{
	public class ConnectionListener : IDisposable
	{
		private static readonly object Sync = new();

		private IHost host;

		public DbConnection DbConnection { get; private set; }
		public EndPoint EndPoint { get; private set; }

		public string DatabaseName => DbConnection.DatabaseName;

		internal ConnectionListener()
		{
		}

		static ConnectionListener()
		{
			ServicePointManager.UseNagleAlgorithm = true;
		}

		internal virtual void Listen(DbConnection listenerDbConnection, EndPoint listenerEndPoint)
		{
			listenerDbConnection = listenerDbConnection ?? throw new ArgumentNullException(nameof(listenerDbConnection));
			listenerEndPoint = listenerEndPoint ?? throw new ArgumentNullException(nameof(listenerEndPoint));
			
			lock (Sync)
			{
				EndPoint = listenerEndPoint;
				DbConnection = listenerDbConnection;
				ConnectionListenerStartup.Listener = this;

				try
				{
					host = Host.CreateDefaultBuilder()
						.ConfigureWebHostDefaults(builder =>
						{
							builder.UseStartup<ConnectionListenerStartup>();
							builder.ConfigureKestrel(opts =>
							{
								opts.AddServerHeader = true;
								opts.Limits.MaxRequestBodySize = null;
								opts.Limits.MaxResponseBufferSize = null;
								opts.Limits.MaxConcurrentConnections = null;
								opts.ListenAnyIP(listenerEndPoint.Port);
							});
							builder.UseUrls(listenerEndPoint.ToWildcardUrl());
						}).ConfigureLogging(logging =>
						{
							logging.ClearProviders();
							logging.AddConsole(opts =>
							{
								opts.LogToStandardErrorThreshold = LogLevel.Error;
							});
						}).Build();

					host.Start();

					Log.Out.Info($"Connection listener on {listenerEndPoint.ToUrl()}");
				}
				catch (Exception err)
				{
					Log.Out.Error($"Failed to listen on {listenerEndPoint.ToUrl()}, {err}");
					throw err;
				}
			}
		}

		public virtual void Dispose()
		{
			host.StopAsync().Wait();
			Log.Out.Info($"Disposed listener on {EndPoint.ToUrl()}");
		}
	}
}