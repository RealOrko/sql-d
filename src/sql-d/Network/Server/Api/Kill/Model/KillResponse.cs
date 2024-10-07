namespace SqlD.Network.Server.Api.Kill.Model;

public class KillResponse
{
    public KillResponse(EndPoint authorityAddress)
    {
        ServerDateTime = DateTime.UtcNow;
        AuthorityAddress = authorityAddress;
        RuntimeVersion = Environment.Version.ToString();
    }

    public string RuntimeVersion { get; }
    public EndPoint AuthorityAddress { get; }
    public DateTime ServerDateTime { get; }
}