namespace SqlD.Network.Server.Api.Db.Model;

public class QueryResponse
{
    public EndPoint EndPoint { get; set; }
    public Query Query { get; set; }
    public string Error { get; set; }
    public StatusCode StatusCode { get; set; }
    public List<List<object>> Rows { get; set; } = new();

    public static QueryResponse Ok(EndPoint endPoint, Query query, List<List<object>> rows)
    {
        return new QueryResponse
        {
            EndPoint = endPoint,
            StatusCode = StatusCode.Ok, 
            Query = query, 
            Rows = rows
        };
    }

    public static QueryResponse Failed(EndPoint endPoint, string err)
    {
        return new QueryResponse
        {
            EndPoint = endPoint,
            StatusCode = StatusCode.Failed, 
            Error = err
        };
    }
}