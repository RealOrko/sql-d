// using System.IO;
// using System.IO.Compression;
// using System.Runtime.InteropServices;
// using System.Threading;
// using SqlD.Extensions.System.Reflection;
//
// namespace SqlD.Process
// {
// 	public class Unpacker
// 	{
// 		private static long isUnpacked;
// 		private static readonly object synchronise = new object();
//
// 		public static void Unpack(bool force = true)
// 		{
// 			if (Interlocked.Read(ref isUnpacked) == 1)
// 				return;
//
// 			lock (synchronise)
// 			{
// 				if (Interlocked.Read(ref isUnpacked) == 1)
// 					return;
//
// 				var slash = Path.DirectorySeparatorChar;
// 				var workingDirectory = $"{typeof(Unpacker).Assembly.GetDirectory()}{slash}";
// 				var processDirectory = $"{workingDirectory}Process{slash}";
// 				var binDirectory = $"{workingDirectory}bin{slash}";
//
// 				if (!force && Directory.Exists(binDirectory))
// 					return;
//
// 				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
// 				{
// 					using (var zipArchive = ZipFile.OpenRead($"{processDirectory}win-x64.zip"))
// 					{
// 						zipArchive.ExtractToDirectory($"{workingDirectory}{slash}bin{slash}win-x64", overwriteFiles: true);
// 					}
// 				}
//
// 				if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
// 				{
// 					using (var zipArchive = ZipFile.OpenRead($"{processDirectory}osx-x64.zip"))
// 					{
// 						zipArchive.ExtractToDirectory($"{workingDirectory}{slash}bin{slash}osx-x64", overwriteFiles: true);
// 					}
// 				}
//
// 				if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
// 				{
// 					using (var zipArchive = ZipFile.OpenRead($"{processDirectory}linux-x64.zip"))
// 					{
// 						zipArchive.ExtractToDirectory($"{workingDirectory}{slash}bin{slash}linux-x64", overwriteFiles: true);
// 					}
// 				}
//
// 				Interlocked.Exchange(ref isUnpacked, 1);
// 			}
// 		}
// 	}
// }