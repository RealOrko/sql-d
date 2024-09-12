namespace SqlD.Exceptions;

public class ConnectionClientCommandException : Exception
{
    public ConnectionClientCommandException(string message) : base(message)
    {
    }
}