using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlD.Configs.Model;

namespace SqlD.Configs;

public static class Configuration
{
    private static Assembly _assembly;
    private static string _settingsFile;
    private static SqlDConfiguration _instance;
    private static readonly object Synchronise = new();

    static Configuration()
    {
        _instance = SqlDConfiguration.Default();
    }

    private static string _assemblyDirectory => Path.GetDirectoryName(new Uri(_assembly.Location).LocalPath);

    public static SqlDConfiguration Instance
    {
        get
        {
            lock (Synchronise)
            {
                return _instance;
            }
        }
    }

    public static SqlDConfiguration Load(Assembly entryAssembly, string settingsFile = "appsettings.json")
    {
        lock (Synchronise)
        {
            _assembly = entryAssembly;
            _settingsFile = settingsFile;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_assemblyDirectory);

            if (File.Exists(Path.Combine(_assemblyDirectory, _settingsFile)))
            {
                builder.AddJsonFile(settingsFile);
                var configuration = builder.Build();
                var section = configuration.GetSection("SqlD");
                _instance = section.Get<SqlDConfiguration>();
            }

            return _instance;
        }
    }

    public static void Update(Assembly entryAssembly, SqlDConfiguration config, string settingsFile = "appsettings.json")
    {
        lock (Synchronise)
        {
            _instance = config;
            _assembly = entryAssembly;
            _settingsFile = settingsFile;
            var settingsFilePath = Path.Combine(_assemblyDirectory, settingsFile);

            var json = File.ReadAllText(settingsFilePath);
            var jsonInstance = JObject.Parse(json);
            jsonInstance["SqlD"] = JObject.Parse(JsonConvert.SerializeObject(config));

            json = JsonConvert.SerializeObject(jsonInstance, Formatting.Indented);
            File.WriteAllText(settingsFilePath, json);
        }
    }

    public static void Reset()
    {
        lock (Synchronise)
        {
            _instance = SqlDConfiguration.Default();
        }
    }
}