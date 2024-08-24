// using System.Diagnostics;
// using SqlD.Logging;
//
// namespace SqlD.Process
// {
// 	public class Command
// 	{
// 		public const bool ENABLE_DEBUG_MODE = false;
//
// 		private static readonly object Synchronise = new object();
//
// 		public static System.Diagnostics.Process Launch(string executable, string arguments, string workingDirectory)
// 		{
// 			lock (Synchronise)
// 			{
// 				var startInfo = new ProcessStartInfo();
// 				startInfo.CreateNoWindow = !ENABLE_DEBUG_MODE;
// 				startInfo.UseShellExecute = ENABLE_DEBUG_MODE;
// 				startInfo.RedirectStandardOutput = !ENABLE_DEBUG_MODE;
// 				startInfo.RedirectStandardError = !ENABLE_DEBUG_MODE;
// 				startInfo.FileName = executable;
// 				startInfo.Arguments = arguments;
// 				startInfo.WorkingDirectory = workingDirectory;
//
// 				Log.Out.Info($"Command Launching: {startInfo.FileName} {startInfo.Arguments} {startInfo.WorkingDirectory}");
//
// 				var process = new System.Diagnostics.Process();
// 				process.StartInfo = startInfo;
// 				process.ErrorDataReceived += (sender, args) => Log.Out.Error(args.Data);
// 				process.OutputDataReceived += (sender, args) => Log.Out.Info(args.Data);
// 				process.EnableRaisingEvents = !ENABLE_DEBUG_MODE;
// 				process.Start();
//
// 				if (!ENABLE_DEBUG_MODE)
// 				{
// 					process.BeginErrorReadLine();
// 					process.BeginOutputReadLine();
// 				}
//
// 				return process;
// 			}
// 		}
// 	}
// }