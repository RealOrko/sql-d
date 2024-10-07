namespace SqlD.Configs.Model;

public class SqlDSettingsForwarding
{
    public const string PRIMARY_STRATEGY = "primary"; // Write nodes are updated first
    public const string SECONDARY_STRATEGY = "secondary"; // Read nodes are updated first
    
    public bool Allowed { get; set; } = true;
    public string Strategy { get; set; } = "Secondary";
}