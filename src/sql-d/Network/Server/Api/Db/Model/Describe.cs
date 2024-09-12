namespace SqlD.Network.Server.Api.Db.Model;

public class Describe
{
    public static readonly IReadOnlyList<string> DescribeColumns = new[] { "cid", "name", "type", "notnull", "dflt_value", "pk" }.ToList();
    public static readonly IReadOnlyList<string> DescribeMasterColumns = new[] { "type", "name", "tbl_name", "sql" }.ToList();
    public string TableName { get; set; }

    public static int IndexOf(string columnName)
    {
        return DescribeColumns.ToList().IndexOf(columnName);
    }
}