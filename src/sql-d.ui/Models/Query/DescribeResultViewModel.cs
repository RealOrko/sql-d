using System.Collections.Generic;
using SqlD.Network.Server.Api.Db.Model;

namespace SqlD.UI.Models.Query;

public class DescribeResultViewModel
{
    public DescribeResultViewModel(string error)
    {
        Error = error;
    }

    public DescribeResultViewModel(DescribeResponse response)
    {
        Table = response.Query.TableName;
        Columns = response.Columns;
        Rows = response.Results;
    }

    public string Error { get; set; }
    public string Table { get; set; }
    public string[] Columns { get; set; }
    public List<List<object>> Rows { get; set; }
}