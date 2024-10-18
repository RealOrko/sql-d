namespace SqlD.Configs.Model;

public class SqlDSettingsReplication
{
    public bool Allowed { get; set; } = false;
    public int Interval { get; set; } = 30;
    public int Delay { get; set; } = 30;
}