using Microsoft.AspNetCore.Mvc;
using SqlD.Logging;

namespace SqlD.Network.Server.Api
{
	internal static class ControllerExtensions
	{
		internal static T Intercept<T>(this ControllerBase controller, Func<T> func)
		{
			try
			{
				return func();
			}
			catch (Exception err)
			{
				Log.Out.Error(err.Message);
				Log.Out.Error(err.StackTrace);
				throw;
			}
		}

		internal static async Task<T> InterceptAsync<T>(this ControllerBase controller, Func<Task<T>> func)
		{
			try
			{
				return await func();
			}
			catch (Exception err)
			{
				Log.Out.Error(err.Message);
				Log.Out.Error(err.StackTrace);
				throw;
			}
		}

	}
}
