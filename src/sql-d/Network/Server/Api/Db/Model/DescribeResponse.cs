namespace SqlD.Network.Server.Api.Db.Model;

public class DescribeResponse
{
    public string Error { get; set; }
    public Describe Query { get; set; }
    public string[] Columns { get; set; }
    public List<List<object>> Results { get; set; }
    public StatusCode StatusCode { get; set; } = StatusCode.Ok;

    public static object Ok(Describe describe, List<List<object>> results)
    {
        return new DescribeResponse
        {
            Query = describe,
            Columns = Describe.DescribeColumns.ToArray(),
            Results = results
        };
    }

    public static object Failed(string error)
    {
        return new DescribeResponse
        {
            Error = error,
            StatusCode = StatusCode.Failed
        };
    }

    public static string GetName(List<object> results)
    {
        return results[Describe.IndexOf("name")].ToString();
    }
}