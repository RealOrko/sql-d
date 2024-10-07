using SqlD.Configuration.Model;
using SqlD.Network;
using SqlD.Network.Client;
using SqlD.Network.Server;
using SqlD.Network.Diagnostics;

namespace SqlD.Tests.Framework.Network
{
	public class WellKnown
	{
		public class EndPoints
		{
			public static EndPoint[] All => new[]
			{
				Alpha,
				Beta,
				Free1,
				Free2,
				Registry
			};

			public static EndPoint Alpha { get; } = EndPoint.Local(9651);
			public static EndPoint Beta { get; } = EndPoint.Local(9652);
			public static EndPoint Free1 { get; } = EndPoint.Local(9653);
			public static EndPoint Free2 { get; } = EndPoint.Local(9654);
			public static EndPoint Registry { get; } = EndPoint.Local(9650);
			public static EndPoint Unreachable { get; } = new EndPoint("1.1.1.1", 9656);
		}

		public class Clients
		{
			public static readonly object Synchronise = new object();

			public static ConnectionClient RegistryClient { get; private set; }
			public static ConnectionClient AlphaClient { get; private set; }
			public static ConnectionClient BetaClient { get; private set; }
			public static ConnectionClient Free1Client { get; private set; }
			public static ConnectionClient Free2Client { get; private set; }

			public static void SetUp()
			{
				lock (Synchronise)
				{
					RegistryClient = Interface.NewClient().ConnectedTo(EndPoints.Registry);
					EndPointMonitor.WaitUntil(RegistryClient.EndPoint, EndPointIs.Up);

					AlphaClient = Interface.NewClient().ConnectedTo(EndPoints.Alpha);
					EndPointMonitor.WaitUntil(AlphaClient.EndPoint, EndPointIs.Up);

					BetaClient = Interface.NewClient().ConnectedTo(EndPoints.Beta);
					EndPointMonitor.WaitUntil(BetaClient.EndPoint, EndPointIs.Up);

					Free1Client = Interface.NewClient().ConnectedTo(EndPoints.Free1);
					EndPointMonitor.WaitUntil(Free1Client.EndPoint, EndPointIs.Up);

					Free2Client = Interface.NewClient().ConnectedTo(EndPoints.Free2);
					EndPointMonitor.WaitUntil(Free2Client.EndPoint, EndPointIs.Up);
				}
			}

			public static void TearDown()
			{
				lock (Synchronise)
				{
					RegistryClient.Dispose();
					EndPointMonitor.WaitUntil(RegistryClient.EndPoint, EndPointIs.Down);

					AlphaClient.Dispose();
					EndPointMonitor.WaitUntil(AlphaClient.EndPoint, EndPointIs.Down);

					BetaClient.Dispose();
					EndPointMonitor.WaitUntil(BetaClient.EndPoint, EndPointIs.Down);

					Free1Client.Dispose();
					EndPointMonitor.WaitUntil(Free1Client.EndPoint, EndPointIs.Down);

					Free2Client.Dispose();
					EndPointMonitor.WaitUntil(Free2Client.EndPoint, EndPointIs.Down);
				}
			}
		}

		public class Listeners
		{
			public static readonly object Synchronise = new object();

			public static ConnectionListener RegistryListener { get; private set; }
			public static ConnectionListener AlphaListener { get; private set; }
			public static ConnectionListener BetaListener { get; private set; }
			public static ConnectionListener Free1Listener { get; private set; }
			public static ConnectionListener Free2Listener { get; private set; }

			public static void SetUp()
			{
				lock (Synchronise)
				{
					RegistryListener = Interface.NewListener().Hosting(typeof(Interface).Assembly, Interface.NewId(), Interface.NewId(), SqlDPragmaModel.Default, EndPoints.Registry);
					EndPointMonitor.WaitUntil(RegistryListener.EndPoint, EndPointIs.Up);

					AlphaListener = Interface.NewListener().Hosting(typeof(Interface).Assembly, Interface.NewId(), Interface.NewId(), SqlDPragmaModel.Default, EndPoints.Alpha);
					EndPointMonitor.WaitUntil(AlphaListener.EndPoint, EndPointIs.Up);

					BetaListener = Interface.NewListener().Hosting(typeof(Interface).Assembly, Interface.NewId(), Interface.NewId(), SqlDPragmaModel.Default, EndPoints.Beta);
					EndPointMonitor.WaitUntil(BetaListener.EndPoint, EndPointIs.Up);

					Free1Listener = Interface.NewListener().Hosting(typeof(Interface).Assembly, Interface.NewId(), Interface.NewId(), SqlDPragmaModel.Default, EndPoints.Free1, EndPoints.Free2);
					EndPointMonitor.WaitUntil(Free1Listener.EndPoint, EndPointIs.Up);

					Free2Listener = Interface.NewListener().Hosting(typeof(Interface).Assembly, Interface.NewId(), Interface.NewId(), SqlDPragmaModel.Default, EndPoints.Free2);
					EndPointMonitor.WaitUntil(Free2Listener.EndPoint, EndPointIs.Up);
				}
			}

			public static void TearDown()
			{
				lock (Synchronise)
				{
					RegistryListener.Dispose();
					EndPointMonitor.WaitUntil(RegistryListener.EndPoint, EndPointIs.Down);

					AlphaListener.Dispose();
					EndPointMonitor.WaitUntil(AlphaListener.EndPoint, EndPointIs.Down);

					BetaListener.Dispose();
					EndPointMonitor.WaitUntil(BetaListener.EndPoint, EndPointIs.Down);

					Free1Listener.Dispose();
					EndPointMonitor.WaitUntil(Free1Listener.EndPoint, EndPointIs.Down);

					Free2Listener.Dispose();
					EndPointMonitor.WaitUntil(Free2Listener.EndPoint, EndPointIs.Down);
				}
			}
		}
	}
}