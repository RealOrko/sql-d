using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlD.Configs.Model;
using SqlD.Logging;

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
            
            Log.Out.Info($"Loading configuration from {_assemblyDirectory}");

            var builder = new ConfigurationBuilder()
                .SetBasePath(_assemblyDirectory);

            var fullSettingsFilePath = Path.Combine(_assemblyDirectory, _settingsFile);
            if (File.Exists(fullSettingsFilePath))
            {
                Log.Out.Info($"Loading configuration from {_assemblyDirectory}");
                builder.AddJsonFile(settingsFile);
                var configuration = builder.Build();
                var section = configuration.GetSection("SqlD");
                _instance = section.Get<SqlDConfiguration>();
                _instance.SetDataDirectory(_assemblyDirectory);
            }
            
            return _instance;
        }
    }

    public static void Update(Assembly entryAssembly, SqlDConfiguration config, string settingsFile = "appsettings.json")
    {
        lock (Synchronise)
        {
            _assembly = entryAssembly;
            _settingsFile = settingsFile;
            
            Log.Out.Info($"Updating configuration from {_assemblyDirectory}");
            
            _instance = config;
            _instance.SetDataDirectory(_assemblyDirectory);
            
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
            Log.Out.Info($"Resetting configuration from {_assemblyDirectory}");
            _instance = SqlDConfiguration.Default();
            _instance.SetDataDirectory(_assemblyDirectory);
        }
    }
}