namespace SqlD.Logging;

public class Log
{
    public static ILogProvider Out = new ConsoleLogger();
}