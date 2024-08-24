// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Reflection;
// using System.Runtime.InteropServices;
// using System.Threading;
// using SqlD.Configuration.Model;
// using SqlD.Exceptions;
// using SqlD.Extensions.System.Reflection;
// using SqlD.Logging;
// using SqlD.Network;
// using SqlD.Network.Diagnostics;
// using SqlD.Network.Server.Api.Registry;
//
// namespace SqlD.Process
// {
// 	public class Service
// 	{
// 		public static System.Diagnostics.Process Start(Assembly startAssembly, SqlDServiceModel service)
// 		{
// 			var executable = string.Empty;
// 			var workingDirectory = string.Empty;
// 			var arguments = ParseArguments(service);
// 			var baseDirectory = startAssembly.GetDirectory();
//
// 			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
//             {
//                 var winRumtimeX64 = "win-x64";
// 				workingDirectory = Path.Combine(baseDirectory, "sql-d.start", winRumtimeX64).Replace("/", "\\");
// 				executable = Path.Combine(workingDirectory, $"SqlD.Start.{winRumtimeX64}.exe").Replace("/", "\\");
//                 if (!File.Exists(executable))
//                 {
//                     throw new Exception($"Could not find '{executable}'. Please install the NuGet 'sql-d.start.win-x64'.");
//                 }
//
// 			}
//
//             if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
//             {
//                 var osxRuntimeX64 = "osx-x64";
// 				workingDirectory = Path.Combine(baseDirectory, "sql-d.start", osxRuntimeX64);
// 				executable = Path.Combine(workingDirectory, $"SqlD.Start.{osxRuntimeX64}");
// 				Command.Launch("/bin/chmod", $"+x {executable}", workingDirectory);
//                 if (!File.Exists(executable))
//                 {
//                     throw new Exception($"Could not find '{executable}'. Please install the NuGet 'sql-d.start.osx-x64'.");
//                 }
//
//             }
//
//             if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
// 			{
//                 var linuxRuntimex64 = "linux-x64";
//                 workingDirectory = Path.Combine(baseDirectory, "sql-d.start", linuxRuntimex64);
// 				executable = Path.Combine(workingDirectory, $"SqlD.Start.{linuxRuntimex64}");
// 				Command.Launch("/bin/chmod", $"+x {executable}", workingDirectory);
//                 if (!File.Exists(executable))
//                 {
//                     throw new Exception($"Could not find '{executable}'. Please install the NuGet 'sql-d.start.linux-x64'.");
//                 }
//
//             }
//
//             Log.Out.Info($"Starting child process {service.Host}:{service.Port}/{service.Database}");
//
// 			var process = Command.Launch(executable, arguments, baseDirectory);
//
// 			WaitUntilServiceIsUp(service);
//
// 			return process;
// 		}
//
// 		private static void WaitUntilServiceIsUp(SqlDServiceModel service)
// 		{
// 			var wait = new ManualResetEvent(false);
// 			using (var endPointMonitor = new EndPointMonitor(service.ToEndPoint()))
// 			{
// 				endPointMonitor.OnUp += (args) =>
// 				{
// 					Log.Out.Info($"Child process {service.Host}:{service.Port}/{service.Database} is up!");
// 					wait.Set();
// 				};
//
// 				try
// 				{
// 					Log.Out.Info($"Waiting {Constants.END_POINT_UP_WAIT_FOR_TIMEOUT.TotalSeconds}s for child process {service.Host}:{service.Port}/{service.Database}");
// 					wait.WaitOne(Constants.END_POINT_UP_WAIT_FOR_TIMEOUT);
//
// 					var client = SqlDStart.NewClient().ConnectedTo(service.ToEndPoint());
// 					if (!client.Ping(service.ToEndPoint()))
// 					{
// 						throw new ProcessStartFailedException($"Failed to launch child process {service.Host}:{service.Port}/{service.Database}");
// 					}
// 				}
// 				catch (Exception err)
// 				{
// 					Log.Out.Error($"Failed to start {service.Host}:{service.Port}/{service.Database} as a separate process");
// 					Log.Out.Error(err.ToString());
// 				}
// 			}
// 		}
//
// 		private static string ParseArguments(SqlDServiceModel service)
// 		{
// 			var nameArgs = $"-n {service.Name}";
// 			var pragmaJournalModelArgs = $"-pj \"{service.Pragma.JournalMode}\"";
// 			var pragmaSynchronousArgs = $"-ps \"{service.Pragma.Synchronous}\"";
// 			var pragmaTempStoreArgs = $"-pt \"{service.Pragma.TempStore}\"";
// 			var pragmaLockingModeArgs = $"-pl \"{service.Pragma.LockingMode}\"";
// 			var pragmaCountChangesArgs = $"-pc \"{service.Pragma.CountChanges}\"";
// 			var pragmaPageSizeArgs = $"-pps \"{service.Pragma.PageSize}\"";
// 			var pragmaCacheSizeArgs = $"-pcs \"{service.Pragma.CacheSize}\"";
// 			var pragmaQueryOnlyArgs = $"-pqo \"{service.Pragma.QueryOnly}\"";
//
// 			var registryArgs = new List<string>();
// 			var availableRegistries = Registry.Get();
// 			if (availableRegistries.Any())
// 			{
// 				foreach (var endPoint in availableRegistries)
// 				{
// 					registryArgs.Add($"-r \"{endPoint.Host}:{endPoint.Port}\"");
// 				}
// 			}
//
// 			var forwardArgs = new List<string>();
// 			if (service.ForwardingTo.Any())
// 			{
// 				foreach (var forwardArg in service.ForwardingTo)
// 				{
// 					forwardArgs.Add($"-f \"{forwardArg.Host}:{forwardArg.Port}\"");
// 				}
// 			}
//
// 			var tagArgs = new List<string>();
// 			if (service.Tags.Any())
// 			{
// 				foreach (var tagArg in service.Tags)
// 				{
// 					tagArgs.Add($"-t \"{tagArg}\"");
// 				}
// 			}
//
// 			return $"{nameArgs} {pragmaPageSizeArgs} {pragmaCacheSizeArgs} {pragmaQueryOnlyArgs} {pragmaCountChangesArgs} {pragmaJournalModelArgs} {pragmaLockingModeArgs} {pragmaSynchronousArgs} {pragmaTempStoreArgs} {string.Join(" ", registryArgs)} -s \"{service.Host}:{service.Port}\" -d \"{service.Database}\" {string.Join(" ", forwardArgs)} {string.Join(" ", tagArgs)} -w";
// 		}
// 	}
// }