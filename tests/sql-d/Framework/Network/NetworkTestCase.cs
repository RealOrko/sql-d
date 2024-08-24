using System.Collections.Generic;
using NUnit.Framework;
using SqlD.Network;
using SqlD.Network.Client;
using SqlD.Network.Server;

namespace SqlD.Tests.Framework.Network
{
	public class NetworkTestCase
	{
		protected static readonly object Synchronise = new object();
		protected static readonly List<ConnectionClient> Clients = new List<ConnectionClient>();
		protected static readonly List<ConnectionListener> Listeners = new List<ConnectionListener>();

		[SetUp]
		public virtual void SetUp()
		{
			Events.ClientCreated += NetworkEventsOnClientCreated();
			Events.ClientDisposed += NetworkEventsOnClientDisposed();
			Events.ListenerCreated += NetworkEventsOnListenerCreated();
			Events.ListenerDisposed += NetworkEventsOnListenerDisposed();

			WellKnown.Listeners.SetUp();
			WellKnown.Clients.SetUp();
		}

		[TearDown]
		public virtual void TearDown()
		{
			WellKnown.Clients.TearDown();
			WellKnown.Listeners.TearDown();

			Events.ClientCreated -= NetworkEventsOnClientCreated();
			Events.ClientDisposed -= NetworkEventsOnClientDisposed();
			Events.ListenerCreated -= NetworkEventsOnListenerCreated();
			Events.ListenerDisposed -= NetworkEventsOnListenerDisposed();
		}

		private static ClientCreatedEvent NetworkEventsOnClientCreated()
		{
			return args =>
			{
				lock (Synchronise)
				{
					Clients.Add(args.Client);
				}
			};
		}

		private static ClientDisposedEvent NetworkEventsOnClientDisposed()
		{
			return args =>
			{
				lock (Synchronise)
				{
					Clients.Remove(args.Client);
				}
			};
		}

		private static ListenerCreatedEvent NetworkEventsOnListenerCreated()
		{
			return args =>
			{
				lock (Synchronise)
				{
					Listeners.Add(args.Listener);
				}
			};
		}

		private static ListenerDisposedEvent NetworkEventsOnListenerDisposed()
		{
			return args =>
			{
				lock (Synchronise)
				{
					Listeners.Remove(args.Listener);
				}
			};
		}
	}
}