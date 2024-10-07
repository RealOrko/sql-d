using System.Collections.Generic;
using SqlD.Network.Server.Api.Db.Model;

namespace SqlD.UI.Models.Query;

public class QueryResultViewModel
{
    public QueryResultViewModel(string error)
    {
        Error = error;
    }

    public QueryResultViewModel(QueryResponse response)
    {
        Query = response.Query.Select;
        Columns = response.Query.Columns;
        Rows = response.Rows;
    }

    public string Error { get; set; }
    public string Query { get; set; }
    public List<string> Columns { get; set; }
    public List<List<object>> Rows { get; set; }
}