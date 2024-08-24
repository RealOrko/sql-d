namespace SqlD.Network
{
	public static class Constants
    {
	    public static TimeSpan END_POINT_UP_WAIT_FOR_TIMEOUT { get; } = TimeSpan.FromSeconds(30);
	    public static TimeSpan END_POINT_MONTIOR_SLEEP_INTERVAL { get; } = TimeSpan.FromMilliseconds(50);
    }
}
