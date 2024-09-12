namespace SqlD.Configs.Model;

public class SqlDServiceModel : SqlDEndPointModel
{
    public string Name { get; set; }
    public string Database { get; set; }
    public List<string> Tags { get; set; } = new();
    public SqlDPragmaModel Pragma { get; set; } = SqlDPragmaModel.Default;
    public List<SqlDForwardingModel> ForwardingTo { get; set; } = new();

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(Database)}: {Database}";
    }
}