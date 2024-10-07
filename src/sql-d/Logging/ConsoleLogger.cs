namespace SqlD.Logging
{
	public class ConsoleLogger : ILogProvider
	{
		public void Debug(string message)
		{
            Console.WriteLine($"SqlD: Debug: {message}");
		}

		public void Info(string message)
		{
            Console.WriteLine($"SqlD: Info: {message}");
		}

		public void InfoInline(string message)
		{
			var msg = $"SqlD: Info: {message}";
            Console.Write(msg);
            Console.SetCursorPosition(Console.CursorLeft - msg.Length, Console.CursorTop);
		}

		public void Warn(string message)
		{
            Console.WriteLine($"SqlD: Warn: {message}");
		}

		public void Error(string message)
		{
            Console.WriteLine($"SqlD: Error: {message}");
		}

		public void Fatal(string message)
		{
            Console.WriteLine($"SqlD: Fatal: {message}");
		}
	}
}