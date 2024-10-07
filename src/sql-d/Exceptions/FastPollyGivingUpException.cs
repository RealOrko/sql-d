namespace SqlD.Exceptions
{
	public class FastPollyGivingUpException : Exception
	{
		public FastPollyGivingUpException(string message) : base(message)
		{
		}
	}
}