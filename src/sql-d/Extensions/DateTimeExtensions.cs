namespace SqlD.Extensions;

public static class DateTimeExtensions
{
    public static string ToSqlLiteString(this DateTime value)
    {
        return $"'{value.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}'";
    }
}