using System.Net;

namespace SqlD.Extensions.System.Net;

public static class HttpExtensions
{
    public static string ToQueryString(this object instance)
    {
        if (instance == null)
            return string.Empty;

        var properties = instance.GetType().GetProperties()
            .OrderBy(x => x.Name)
            .Where(p => p.GetValue(instance, null) != null)
            .Select(p => p.Name + "=" + WebUtility.UrlEncode(p.GetValue(instance, null).ToString()));

        return "?" + string.Join("&", properties.ToArray());
    }
}