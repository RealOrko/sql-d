using System.Text;
using SqlD.Exceptions;
using SqlD.Extensions.Discovery;

namespace SqlD.Extensions;

internal static class TableExtensions
{
    internal static string GetCreateTable(this Type type)
    {
        var result = new StringBuilder();

        var tableName = AttributeDiscovery.GetTableName(type);
        result.AppendLine($"CREATE TABLE IF NOT EXISTS {tableName}");
        result.AppendLine("(");

        var columns = new List<string>();

        var properties = PropertyDiscovery.GetProperties(type);
        var idProperty = PropertyDiscovery.GetProperty("Id", type);
        if (idProperty != null)
        {
            if (idProperty.PropertyType != typeof(int) && idProperty.PropertyType != typeof(long))
                throw new InvalidSqlLiteIdentityMappingException("Invalid type for 'Id' expected Int32 or Int64.");
            columns.Add($"{idProperty.Name.ToUpper()} INTEGER PRIMARY KEY AUTOINCREMENT");
        }

        foreach (var property in properties)
        {
            if (property.Name == idProperty.Name) continue;
            var columnName = AttributeDiscovery.GetColumnName(property);
            var columnType = AttributeDiscovery.GetColumnType(property);
            var columnNullable = AttributeDiscovery.GetColumnNullable(property);
            columns.Add($"{columnName} {columnType.ToString().ToUpper()} {columnNullable}");
        }

        result.AppendLine(string.Join(",\r\n", columns));
        result.AppendLine(");");
        result.AppendLine($"CREATE UNIQUE INDEX IF NOT EXISTS {tableName}_ID_INDEX ON {tableName} (ID);");

        foreach (var property in properties)
        {
            if (property.Name == idProperty.Name) continue;
            var columnName = AttributeDiscovery.GetColumnName(property);
            var indexName = AttributeDiscovery.GetIndexName(property);
            var indexUnique = AttributeDiscovery.GetIndexUnique(property);
            var additionalColumns = AttributeDiscovery.GetIndexAdditionalColumns(property);
            if (!string.IsNullOrEmpty(additionalColumns))
                columnName = string.Join(",", columnName, additionalColumns);
            result.AppendLine($"CREATE {indexUnique} INDEX IF NOT EXISTS {tableName}_{indexName}_INDEX ON {tableName} ({columnName});");
        }

        return result.ToString();
    }

    internal static string GetDropTable(this Type type)
    {
        var tableName = AttributeDiscovery.GetTableName(type);
        return $"DROP TABLE IF EXISTS {tableName};";
    }
}