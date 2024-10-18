using System.Collections.Generic;
using SqlD.Network;
using SqlD.Network.Server.Api.Db.Model;

namespace SqlD.UI.Models.Query;

public class CommandResultViewModel
{
    public CommandResultViewModel(EndPoint endPoint, string error)
    {
        EndPoint = endPoint;
        Error = error;
    }

    public CommandResultViewModel(CommandResponse response)
    {
        EndPoint = response.EndPoint;
        Status = response.StatusCode;
        ScalarResults = response.ScalarResults;
    }

    public EndPoint EndPoint { get; set; }
    public string Error { get; set; }
    public StatusCode Status { get; set; }
    public List<long> ScalarResults { get; set; }
}