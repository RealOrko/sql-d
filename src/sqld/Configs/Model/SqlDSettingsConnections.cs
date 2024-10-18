namespace SqlD.Configs.Model;

public class SqlDSettingsConnections
{
    public const string SINGLETON_STRATEGY = "singleton"; // New connection for each request
    public const string FACTORY_STRATEGY = "factory"; // Re-uses same connection, less locks
    
    public string Strategy { get; set; } = "single";
}
