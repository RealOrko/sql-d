namespace SqlD.Configs.Model;

public class SqlDPragmaModel
{
    public static SqlDPragmaModel Default = new()
    {
        TempStore = "OFF",
        CountChanges = "OFF",
        LockingMode = "OFF",
        Synchronous = "OFF",
        JournalMode = "OFF",
        PageSize = "65536",
        CacheSize = "10000",
        QueryOnly = "OFF"
    };

    public string JournalMode { get; set; }
    public string Synchronous { get; set; }
    public string TempStore { get; set; }
    public string LockingMode { get; set; }
    public string CountChanges { get; set; }
    public string PageSize { get; set; }
    public string CacheSize { get; set; }
    public string QueryOnly { get; set; }
}