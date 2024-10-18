namespace SqlD.Exceptions;

public class DbConnectionFailedException(string message, Exception innerException) : Exception(message, innerException);