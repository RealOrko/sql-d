namespace SqlD.Configs.Model;

public class SqlDReplication
{
    public bool Allowed { get; set; } = false;
    public int Interval { get; set; } = 30;
}