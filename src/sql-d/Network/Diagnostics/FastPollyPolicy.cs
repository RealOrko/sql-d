using SqlD.Exceptions;
using SqlD.Logging;

namespace SqlD.Network.Diagnostics;

public class FastPollyPolicy(Type exceptionType)
{
    private Func<int, TimeSpan> _calculateCurrentTry;
    private int _totalNumberOfRetries;

    public FastPollyPolicy WaitAndRetryAsync(int numberOfRetries, Func<int, TimeSpan> calculateNextWait)
    {
        _totalNumberOfRetries = numberOfRetries;
        _calculateCurrentTry = calculateNextWait;
        return this;
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> callback)
    {
        var currentTry = 0;
        var currentFailures = 0;

        while (true)
            try
            {
                return await callback();
            }
            catch (Exception err)
            {
                // ReSharper disable once UseMethodIsInstanceOfType
                if (exceptionType.IsAssignableFrom(err.GetType()))
                {
                    currentTry += 1;
                    currentFailures += 1;
                    Log.Out.Warn($"FastPolly is retrying. CurrentTry={currentTry}, TotalRetries={_totalNumberOfRetries}");
                    Thread.Sleep(_calculateCurrentTry(currentTry));
                    if (currentTry >= _totalNumberOfRetries)
                    {
                        var error = $"FastPolly is giving up. TotalRetries={_totalNumberOfRetries}, CurrentTry={currentTry}, CurrentFailures={currentFailures}, Exception={err}";
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