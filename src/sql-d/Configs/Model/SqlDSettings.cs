namespace SqlD.Configs.Model;

public class SqlDSettings
{
    public SqlDSettingsReplication Replication { get; set; } = new();
    public SqlDSettingsForwarding Forwarding { get; set; } = new();
}