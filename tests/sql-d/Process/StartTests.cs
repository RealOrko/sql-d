// using System;
// using System.Threading;
// using NUnit.Framework;
// using SqlD.Configuration.Model;
// using SqlD.Network.Diagnostics;
// using SqlD.Tests.Framework.Network;
// using R = SqlD.Network.Server.Api.Registry.Registry;
//
// namespace SqlD.Tests.Process
// {
// 	[TestFixture]
// 	public class StartTests : NetworkTestCase
// 	{
// 		[Test]
// 		public void ShouldBeAbleToLaunchServiceInASeparateProcess()
// 		{
// 			R.GetOrAdd(WellKnown.Listeners.AlphaListener.EndPoint);
//
// 			var service = new SqlDServiceModel()
// 			{
// 				Name = "separate-process", 
// 				Database = SqlDStart.NewId(),
// 				Port = WellKnown.EndPoints.Free2.Port - 100,
// 				Host = WellKnown.EndPoints.Free2.Host,
// 				Pragma = SqlDPragmaModel.Default
// 			};
//
// 			using (var process = SqlD.Process.Service.Start(typeof(StartTests).Assembly, service))
// 			{
// 				var wait = new ManualResetEvent(false);
// 				using (var endPointMonitor = new EndPointMonitor(service.ToEndPoint()))
// 				{
// 					endPointMonitor.OnUp += (args) => wait.Set();
// 					wait.WaitOne(TimeSpan.FromSeconds(30));
// 				}
// 				process.Kill();
// 			}
// 		}
// 	}
// }