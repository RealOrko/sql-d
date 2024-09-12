namespace SqlD.Network.Diagnostics;

public class EndPointArgs : EventArgs
{
    public EndPointArgs(EndPoint endPoint)
    {
        this.EndPoint = endPoint;
    }

    public EndPoint EndPoint { get; }
}