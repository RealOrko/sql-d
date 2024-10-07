namespace SqlD.Network.Diagnostics
{
	public class EndPointArgs : EventArgs
	{
		private readonly EndPoint endPoint;

		public EndPointArgs(EndPoint endPoint)
		{
			this.endPoint = endPoint;
		}

		public EndPoint EndPoint => endPoint;
	}
}
