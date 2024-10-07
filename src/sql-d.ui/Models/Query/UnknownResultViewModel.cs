using System.Collections.Generic;

namespace SqlD.UI.Models.Query;

public class UnknownResultViewModel
{
    public UnknownResultViewModel(string query)
    {
        Message = $"Unable to parse '{query}' into anything meaningful, please try again";
    }

    public string Message { get; }

    public List<string> Examples { get; } = new()
    {
        "api/query/describe sqlite_master",
        "api/query/select * from sqlite_master"
    };
}