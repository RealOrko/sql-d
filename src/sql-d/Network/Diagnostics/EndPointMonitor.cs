using System.Diagnostics;
using SqlD.Exceptions;

namespace SqlD.Network.Diagnostics
{
	public enum EndPointIs
	{
		Up = 0,
		Down = 1
	}

	public class EndPointMonitor : IDisposable
	{
		private long isUp;
		private long isRunning;
		private bool isDisposed;
		private Task monitorTask;

		public EndPoint EndPoint { get; }

		public event EndPointStateChangedEvent OnUp;
		public event EndPointStateChangedEvent OnDown;

		public EndPointMonitor(EndPoint endPoint)
		{
			this.isUp = 0;
			this.isRunning = 1;
			this.EndPoint = endPoint;
			this.monitorTask = Task.Factory.StartNew(async () => await MonitorEndPoint());
		}

		public void WaitUntil(TimeSpan timeout, EndPointIs upOrDown)
		{
			var stopWatch = Stopwatch.StartNew();
			try
			{
				while (Interlocked.Read(ref isRunning) == 1)
				{
					try
					{
						if (Interlocked.Read(ref isUp) == (upOrDown == EndPointIs.Up ? 1 : 0))
						{
							break;
						}

						if (stopWatch.Elapsed > timeout)
						{
							throw new EndPointMonitorWaitTimeoutException($"{EndPoint} failed to come up in {stopWatch.Elapsed.TotalSeconds} second(s).");
						}
					}
					finally
					{
						if (Interlocked.Read(ref isUp) != (upOrDown == EndPointIs.Up ? 1 : 0))
						{
							Thread.Sleep(Constants.END_POINT_MONTIOR_SLEEP_INTERVAL);
						}
					}
				}
			}
			finally
			{
				stopWatch.Stop();
			}
		}

		private async Task MonitorEndPoint()
		{
			while (Interlocked.Read(ref isRunning) == 1)
			{
				try
				{
					var client = Interface.NewClient().ConnectedTo(EndPoint);
					var pingResult = await client.PingAsync();
					Interlocked.Exchange(ref isUp, pingResult ? 1 : 0);
					DoEvents();
				}
				finally
				{
					if (Interlocked.Read(ref isRunning) != 1)
					{
						Thread.Sleep(Constants.END_POINT_MONTIOR_SLEEP_INTERVAL);
					}
				}
			}
		}

		protected virtual void OnIsUp(EndPointArgs args)
		{
			OnUp?.Invoke(args);
		}

		protected virtual void OnIsDown(EndPointArgs args)
		{
			OnDown?.Invoke(args);
		}

		public EndPointMonitor DoEvents()
		{
			if (Interlocked.Read(ref isUp) == 1)
				OnIsUp(new EndPointArgs(EndPoint));
			else 
				OnIsDown(new EndPointArgs(EndPoint));
			return this;
		}

		public void Dispose()
		{
			if (isDisposed)
				return;

			try
			{
				OnUp = null;
				OnDown = null;
				Interlocked.Exchange(ref isRunning, 0);
				try
				{
					monitorTask?.Wait();
				}
				catch
				{
					monitorTask?.Dispose();
					monitorTask = null;
				}
			}
			finally
			{
				isDisposed = true;
			}
		}

		public static void WaitUntil(EndPoint endPoint, EndPointIs upOrDown)
		{
			var endPointMonitor = new EndPointMonitor(endPoint);
			try
			{
				endPointMonitor.WaitUntil(Constants.END_POINT_UP_WAIT_FOR_TIMEOUT, upOrDown);
			}
			finally
			{
				endPointMonitor.Dispose();
			}
		}
	}
}