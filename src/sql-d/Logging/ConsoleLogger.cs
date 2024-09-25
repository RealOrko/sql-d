namespace SqlD.Logging;

public class ConsoleLogger : ILogProvider
{
    private static string Header => $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}: SqlD:";
    
    public void Debug(string message)
    {
        if (IsLogLevel(LogLevel.Debug))
            Console.WriteLine($"{Header} Debug: {message}");
    }

    public void Info(string message)
    {
        if (IsLogLevel(LogLevel.Info))
            Console.WriteLine($"{Header} Info: {message}");
    }

    public void Warn(string message)
    {
        if (IsLogLevel(LogLevel.Warn))
            Console.WriteLine($"{Header} Warn: {message}");
    }

    public void Error(string message)
    {
        if (IsLogLevel(LogLevel.Error))
            Console.WriteLine($"{Header} Error: {message}");
    }

    public void Fatal(string message)
    {
        if (IsLogLevel(LogLevel.Fatal))
            Console.WriteLine($"{Header} Fatal: {message}");
    }

    private bool IsLogLevel(LogLevel logLevel)
    {
        if (string.IsNullOrEmpty(Configs.Configuration.Instance.LogLevel))
        {
            return true;
        }
        else
        {
            var logLevelSetting = ParseEnum<LogLevel>(Configs.Configuration.Instance.LogLevel);
            if (logLevelSetting == LogLevel.Debug)
            {
                switch (logLevel)
                {
                    case LogLevel.Debug:
                    case LogLevel.Info:
                    case LogLevel.Warn:
                    case LogLevel.Error:
                    case LogLevel.Fatal:
                        return true;
                    default:
                        return false;
                }                
            }
            if (logLevelSetting == LogLevel.Info)
            {
                switch (logLevel)
                {
                    case LogLevel.Info:
                    case LogLevel.Warn:
                    case LogLevel.Error:
                    case LogLevel.Fatal:
                        return true;
                    default:
                        return false;
                }                
            }
            if (logLevelSetting == LogLevel.Warn)
            {
                switch (logLevel)
                {
                    case LogLevel.Warn:
                    case LogLevel.Error:
                    case LogLevel.Fatal:
                        return true;
                    default:
                        return false;
                }                
            }
            if (logLevelSetting == LogLevel.Error)
            {
                switch (logLevel)
                {
                    case LogLevel.Error:
                    case LogLevel.Fatal:
                        return true;
                    default:
                        return false;
                }                
            }
            if (logLevelSetting == LogLevel.Fatal)
            {
                switch (logLevel)
                {
                    case LogLevel.Fatal:
                        return true;
                }                
            }
        }
        return true;
    }
    
    private static T ParseEnum<T>(string value)
    {
        return (T) Enum.Parse(typeof(T), value, true);
    }
}