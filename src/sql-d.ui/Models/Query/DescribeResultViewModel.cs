using System.Collections.Generic;
using SqlD.Network;
using SqlD.Network.Server.Api.Db.Model;

namespace SqlD.UI.Models.Query;

public class DescribeResultViewModel
{
    public DescribeResultViewModel(EndPoint endPoint, string error)
    {
        EndPoint = endPoint;
        Error = error;
    }

    public DescribeResultViewModel(DescribeResponse response)
    {
        EndPoint = response.EndPoint;
        Table = response.Query.TableName;
        Columns = response.Columns;
        Rows = response.Results;
    }

    public EndPoint EndPoint { get; set; }
    public string Error { get; set; }
    public string Table { get; set; }
    public string[] Columns { get; set; }
    public List<List<object>> Rows { get; set; }
}