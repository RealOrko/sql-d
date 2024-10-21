namespace SqlD.Network.Diagnostics;

public class EndPointArgs(EndPoint endPoint) : EventArgs
{
    public EndPoint EndPoint { get; } = endPoint;
}