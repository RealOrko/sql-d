namespace SqlD.Network.Server.Api.Unregister.Model;

public class UnregisterResponse
{
    public UnregisterResponse(EndPoint authorityAddress)
    {
        ServerDateTime = DateTime.UtcNow;
        AuthorityAddress = authorityAddress;
        RuntimeVersion = Environment.Version.ToString();
    }

    public string RuntimeVersion { get; }
    public EndPoint AuthorityAddress { get; }
    public DateTime ServerDateTime { get; }
}