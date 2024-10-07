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

    private static string _assemblyDirectory => Path.GetDirectoryName(new Uri(_assembly.Location).LocalPath);

    public static SqlDConfiguration Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            
            lock (Synchronise)
            {
                if (_instance != null)
                    return _instance;
                
                LoadInstance();
                return _instance;
            }
        }
    }

    public static void SetAssembly(Assembly assembly)
    {
        if (_assembly != null)
            throw new InvalidOperationException("The configuration assembly has already been set.");
        _assembly = assembly;
        // This causes null refs
        //Log.Out.Info($"Setting start assembly to {assembly.FullName}");
    }

    public static void SetSettingsFile(string settingsFile)
    {
        if (_settingsFile != null)
            throw new InvalidOperationException("The configuration settings file has already been set.");
        _settingsFile = settingsFile;
        // This causes null refs
        //Log.Out.Info($"Setting start settings file to {_settingsFile}");
    } 
    
    private static void LoadInstance()
    {
        lock (Synchronise)
        {
            // This causes stack overflows
            //Log.Out.Info($"Loading configuration from {_assemblyDirectory}");
            var builder = new ConfigurationBuilder()
                .SetBasePath(_assemblyDirectory);

            var fullSettingsFilePath = Path.Combine(_assemblyDirectory, _settingsFile);
            if (File.Exists(fullSettingsFilePath))
            {
                // This causes stack overflows
                //Log.Out.Info($"Loading configuration from {_assemblyDirectory}");
                builder.AddJsonFile(_settingsFile);
                var configuration = builder.Build();
                var section = configuration.GetSection("SqlD");
                _instance = section.Get<SqlDConfiguration>();
                _instance.SetDataDirectory(_assemblyDirectory);
            }
        }
    }

    public static void Update(SqlDConfiguration config)
    {
        lock (Synchronise)
        {
            Log.Out.Info($"Updating configuration from {_assemblyDirectory}");
            
            _instance = config;
            _instance.SetDataDirectory(_assemblyDirectory);
            
            var settingsFilePath = Path.Combine(_assemblyDirectory, _settingsFile);
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
            _instance = null;
        }
    }
}