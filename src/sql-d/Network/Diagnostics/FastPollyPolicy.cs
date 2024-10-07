using SqlD.Exceptions;
using SqlD.Logging;

namespace SqlD.Network.Diagnostics
{
	public class FastPollyPolicy
	{
		private int totalNumberOfRetries = 0;
		private readonly Type exceptionType = null;
		private Func<int, TimeSpan> calculateCurrentTry;

		public FastPollyPolicy(Type exceptionType)
		{
			this.exceptionType = exceptionType;
		}

		public FastPollyPolicy WaitAndRetryAsync(int numberOfRetries, Func<int, TimeSpan> calculateNextWait)
		{
			this.totalNumberOfRetries = numberOfRetries;
			this.calculateCurrentTry = calculateNextWait;
			return this;
		}

		public async Task<T> ExecuteAsync<T>(Func<Task<T>> callback)
		{
			var currentTry = 0;
			var currentFailures = 0;

			while (true)
			{
				try
				{
					return await callback();
				}
				catch (Exception err)
				{
					// ReSharper disable once UseMethodIsInstanceOfType
					if (exceptionType.IsAssignableFrom(err.GetType()))
					{
						currentTry = currentTry + 1;
						currentFailures = currentFailures + 1;
						Log.Out.Warn($"FastPolly is retrying. CurrentTry={currentTry}, TotalRetries={totalNumberOfRetries}");
						Thread.Sleep(calculateCurrentTry(currentTry));
						if (currentTry >= totalNumberOfRetries)
						{
							var error = $"FastPolly is giving up. TotalRetries={totalNumberOfRetries}, CurrentTry={currentTry}, CurrentFailures={currentFailures}, Exception={err}";
							Log.Out.Error(error);
							throw new FastPollyGivingUpException(error);
						}
					}
					else
					{
						throw;
					}
				}
			}
		}
	}
}