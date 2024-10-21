namespace SqlD.Network.Server.Api.Db.Model;

public class Query
{
    public string Select { get; set; }
    public List<string> Columns { get; set; } = new();
    public List<string> Properties { get; set; } = new();
}