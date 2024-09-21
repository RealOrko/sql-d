using System.Collections.Generic;
using SqlD.Network;
using SqlD.Network.Server.Api.Db.Model;

namespace SqlD.UI.Models.Query;

public class QueryResultViewModel
{
    public QueryResultViewModel(EndPoint endPoint, string error)
    {
        EndPoint = endPoint;
        Error = error;
    }

    public QueryResultViewModel(QueryResponse response)
    {
        EndPoint = response.EndPoint;
        Query = response.Query.Select;
        Columns = response.Query.Columns;
        Rows = response.Rows;
    }

    public EndPoint EndPoint { get; set; }
    public string Error { get; set; }
    public string Query { get; set; }
    public List<string> Columns { get; set; }
    public List<List<object>> Rows { get; set; }
}