namespace SqlD.Configs.Model;

public class SqlDSettings
{
    public SqlDSettingsConnections Connections { get; set; } = new();
    public SqlDSettingsReplication Replication { get; set; } = new();
    public SqlDSettingsForwarding Forwarding { get; set; } = new();
}