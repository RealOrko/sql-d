namespace SqlD.Network.Server.Api.Db.Model;

public class CommandResponse
{
    public EndPoint EndPoint { get; set; }
    public List<long> ScalarResults { get; set; }
    public StatusCode StatusCode { get; set; }
    public string Error { get; set; }

    public static CommandResponse Ok(EndPoint endPoint)
    {
        return new CommandResponse
        {
            EndPoint = endPoint,
            StatusCode = StatusCode.Ok
        };
    }

    public static CommandResponse Ok(EndPoint endPoint, List<long> scalarResults)
    {
        return new CommandResponse
        {
            EndPoint = endPoint,
            ScalarResults = scalarResults,
            StatusCode = StatusCode.Ok
        };
    }

    public static CommandResponse Failed(EndPoint endPoint, string error)
    {
        return new CommandResponse
        {
            EndPoint = endPoint,
            Error = error,
            StatusCode = StatusCode.Failed
        };
    }
}