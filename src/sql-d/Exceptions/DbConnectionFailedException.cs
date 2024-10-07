namespace SqlD.Exceptions
{
	public class DbConnectionFailedException : Exception
	{
		public DbConnectionFailedException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}