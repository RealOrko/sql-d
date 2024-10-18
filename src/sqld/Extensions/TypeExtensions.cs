using System.Data;

namespace SqlD.Extensions;

public static class TypeExtensions
{
    public static DbType ToDbType(this Type type)
    {
        switch (type.FullName)
        {
            case "System.Int64":
                return DbType.Int64;
            case "System.Int32":
                return DbType.Int32;
            case "System.Int16":
                return DbType.Int16;
            case "System.Decimal":
                return DbType.Decimal;
            case "System.Double":
                return DbType.Double;
            case "System.Boolean":
                return DbType.Boolean;
            case "System.String":
                return DbType.String;
            case "System.DateTime":
                return DbType.DateTime;
            default:
                return DbType.Object;
        }
    }
}