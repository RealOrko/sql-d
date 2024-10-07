namespace SqlD.Network.Server.Api.Db.Model;

public class DescribeResponse
{
    public EndPoint EndPoint { get; set; }
    public string Error { get; set; }
    public Describe Query { get; set; }
    public string[] Columns { get; set; }
    public List<List<object>> Results { get; set; }
    public StatusCode StatusCode { get; set; } = StatusCode.Ok;

    public static object Ok(EndPoint endPoint, Describe describe, List<List<object>> results)
    {
        return new DescribeResponse
        {
            EndPoint = endPoint,
            Query = describe,
            Columns = Describe.DescribeColumns.ToArray(),
            Results = results
        };
    }

    public static object Failed(EndPoint endPoint, string error)
    {
        return new DescribeResponse
        {
            EndPoint = endPoint,
            Error = error,
            StatusCode = StatusCode.Failed
        };
    }

    public static string GetName(List<object> results)
    {
        return results[Describe.IndexOf("name")].ToString();
    }
}