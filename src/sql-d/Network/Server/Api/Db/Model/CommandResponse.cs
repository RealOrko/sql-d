namespace SqlD.Network.Server.Api.Db.Model
{
	public class CommandResponse
	{
		public List<long> ScalarResults { get; set; }
		public StatusCode StatusCode { get; set; }
		public string Error { get; set; }

		public static CommandResponse Ok()
		{
			return new CommandResponse {StatusCode = StatusCode.Ok};
		}

		public static CommandResponse Ok(List<long> scalarResults)
		{
			return new CommandResponse
			{
				ScalarResults = scalarResults,
				StatusCode = StatusCode.Ok
			};
		}

		public static CommandResponse Failed(string error)
		{
			return new CommandResponse
			{
				Error = error,
				StatusCode = StatusCode.Failed
			};
		}
	}
}