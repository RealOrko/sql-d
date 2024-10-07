namespace SqlD.Extensions;

public static class StringExtensions
{
    public static DateTime FromSqlLiteString(this string value)
    {
        return DateTime.Parse(value);
    }
}