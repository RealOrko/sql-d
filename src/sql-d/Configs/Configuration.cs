using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlD.Configs.Model;

namespace SqlD.Configs
{
	public static class Configuration
	{
		private static Assembly _assembly;
		private static string _settingsFile;
		private static SqlDConfiguration _instance;
		private static readonly object Sync = new();
		private static string _assemblyDirectory => Path.GetDirectoryName(new Uri(_assembly.Location).LocalPath);

		internal static ManualResetEvent ConfigReady { get; } = new(false);

		static Configuration()
		{
			_instance = SqlDConfiguration.Default;
			ConfigReady.Set();
		}
		
		public static SqlDConfiguration Instance
		{
			get
			{
				lock (Sync)
				{
					ConfigReady.WaitOne();
					return _instance;
				}
			}
		}

		public static SqlDConfiguration Load(Assembly entryAssembly, string settingsFile = "appsettings.json")
		{
			lock (Sync)
            {
	            _assembly = entryAssembly;
	            _settingsFile = settingsFile;
	            
                var builder = new ConfigurationBuilder()
                    .SetBasePath(_assemblyDirectory);

                if (File.Exists(_settingsFile))
                {
                    builder.AddJsonFile(settingsFile);
                    var configuration = builder.Build();
                    var section = configuration.GetSection("SqlD");
                    _instance = section.Get<SqlDConfiguration>();
                    ConfigReady.Set();
                    return _instance;
                }

                _instance = SqlDConfiguration.Default;
                ConfigReady.Set();
                return _instance;
            }
		}

		public static void Update(Assembly entryAssembly, SqlDConfiguration config, string settingsFile = "appsettings.json")
		{
			lock (Sync)
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
	}
}