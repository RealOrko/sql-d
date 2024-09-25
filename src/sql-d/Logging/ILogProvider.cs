namespace SqlD.Logging;

public interface ILogProvider
{
    void Debug(string message);
    void Info(string message);
    void Warn(string message);
    void Error(string message);
    void Fatal(string message);
}