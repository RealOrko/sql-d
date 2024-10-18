namespace SqlD.Network.Server.Api.Registry.Model;

public class Registration
{
    public string Name { get; set; }
    public string Database { get; set; }
    public EndPoint Source { get; set; }
    public string[] Tags { get; set; }
}