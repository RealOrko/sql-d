namespace SqlD.Network.Server.Api.Db.Model
{
	public class QueryResponse
	{
		public Query Query { get; set; }
		public string Error { get; set; }
		public StatusCode StatusCode { get; set; }
		public List<List<object>> Rows { get; set; } = new List<List<object>>();

		public static QueryResponse Ok(Query query, List<List<object>> rows)
		{
			return new QueryResponse { StatusCode = StatusCode.Ok, Query = query, Rows = rows };
		}

		public static QueryResponse Failed(string err)
		{
			return new QueryResponse { StatusCode = StatusCode.Failed, Error = err };
		}
	}
}