using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlD.Configuration.Model;
using SqlD.Extensions.System.Reflection;

namespace SqlD.Configuration
{
	public class SqlDConfig
	{
		private static readonly object Synchronise = new object();

		public static SqlDConfiguration Get(Assembly entryAssembly, string settingsFile = "appsettings.json")
		{
			lock (Synchronise)
            {
                var assemblyCodeBase = new Uri(entryAssembly.CodeBase);
				var workingDirectory = Path.GetDirectoryName(assemblyCodeBase.LocalPath);

                var builder = new ConfigurationBuilder()
                    .SetBasePath(workingDirectory);

                if (File.Exists("appsettings.json"))
                {
                    builder.AddJsonFile(settingsFile);
                    var configuration = builder.Build();
                    var section = configuration.GetSection("SqlD");
                    var config = section.Get<SqlDConfiguration>();
                    return config;
                }

                return SqlDConfiguration.Default;
            }
		}

		public static void Set(Assembly entryAssembly, SqlDConfiguration config, string settingsFile = "appsettings.json")
		{
			lock (Synchronise)
			{
				var workingDirectory = entryAssembly.GetDirectory();
				var settingsFilePath = Path.Combine(workingDirectory, settingsFile);

				var json = File.ReadAllText(settingsFilePath);
				var jsonInstance = JObject.Parse(json);
				jsonInstance["SqlD"] = JObject.Parse(JsonConvert.SerializeObject(config));

				json = JsonConvert.SerializeObject(jsonInstance, Formatting.Indented);
				File.WriteAllText(settingsFilePath, json);
			}
		}
	}
}