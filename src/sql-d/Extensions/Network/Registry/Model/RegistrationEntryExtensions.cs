using SqlD.Extensions.Threading;
using SqlD.Network.Server.Api.Registry.Model;

namespace SqlD.Extensions.Network.Registry.Model
{
    public static class RegistrationEntryExtensions
    {
	    public static T[] Convert<T>(this IEnumerable<RegistryEntry> services, Func<RegistryEntry, T> convert)
	    {
		    return services.ForEachTask(convert).ToArray();
	    }

	    public static async Task<T[]> ConvertAsync<T>(this IEnumerable<RegistryEntry> services, Func<RegistryEntry, Task<T>> convert)
	    {
		    return (await services.ForEachTaskAsync(convert)).ToArray();
	    }
	}
}
