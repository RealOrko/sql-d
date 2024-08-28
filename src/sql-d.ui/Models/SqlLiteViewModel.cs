using SqlD.UI.Models.Query;

namespace SqlD.UI.Models;

public class SqlLiteViewModel
{
    public string Server { get; set; }
    public string Query { get; set; }
    public QueryResultViewModel QueryResult { get; set; }
    public CommandResultViewModel CommandResult { get; set; }
    public DescribeResultViewModel DescribeResult { get; set; }
}