using System.Net;
using System.Reflection;
using SqlD.Logging;

namespace SqlD.Network.Server
{
	public class ConnectionListener : IDisposable
	{
		private static readonly object Synchronise = new object();

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

		internal virtual ConnectionListener Listen(Assembly startAssembly, DbConnection listenerDbConnection, EndPoint listenerEndPoint, EndPoint[] forwardEndPoints = null)
		{
			listenerDbConnection = listenerDbConnection ?? throw new ArgumentNullException(nameof(listenerDbConnection));
			listenerEndPoint = listenerEndPoint ?? throw new ArgumentNullException(nameof(listenerEndPoint));

			lock (Synchronise)
			{
				EndPoint = listenerEndPoint;
				DbConnection = listenerDbConnection;

			    ConnectionListenerStartup.StartAssembly = startAssembly;
			    ConnectionListenerStartup.DbConnection = listenerDbConnection;
                ConnectionListenerStartup.ListenerAddress = listenerEndPoint;
				ConnectionListenerStartup.ForwardAddresses = forwardEndPoints;

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

			return this;
		}

		public virtual void Dispose()
		{
			ConnectionListenerFactory.Remove(this);
			host.StopAsync().Wait();
			Log.Out.Info($"Disposed listener on {EndPoint.ToUrl()}");
		}
	}
}