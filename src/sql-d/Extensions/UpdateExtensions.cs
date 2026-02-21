using Microsoft.Data.Sqlite;
using System.Text;
using SqlD.Extensions.Discovery;

namespace SqlD.Extensions;

internal static class UpdateExtensions
{
    [Obsolete("This does not use prepared statements, please do not use")]
    internal static string GetUpdate(this object instance)
    {
        var result = new StringBuilder();

        var type = instance.GetType();

        var tableName = AttributeDiscovery.GetTableName(type);
        result.AppendLine($"UPDATE {tableName}");
        result.AppendLine("SET");

        var columns = new List<string>();

        var properties = PropertyDiscovery.GetProperties(type);

        var idProperty = PropertyDiscovery.GetProperty("Id", type);
        foreach (var property in properties)
        {
            if (property.Name == idProperty.Name) continue;
            var columnName = AttributeDiscovery.GetColumnName(property);
            var columnValue = ValueDiscovery.GetValue(instance, property);
            columns.Add($"{columnName} = {columnValue}");
        }

        result.AppendLine(string.Join(",\r\n", columns));

        result.AppendLine("WHERE");
        result.AppendLine($"Id = {ValueDiscovery.GetValue(instance, idProperty)}");

        return result.ToString();
    }

    internal static SqliteCommand GetUpdatePrepared(this object instance, SqliteConnection connection)
    {
        var command = connection.CreateCommand();

        var result = new StringBuilder();

        var type = instance.GetType();

        var tableName = AttributeDiscovery.GetTableName(type);
        result.AppendLine($"UPDATE {tableName}");
        result.AppendLine("SET");

        var columns = new List<string>();

        var properties = PropertyDiscovery.GetProperties(type);
        var paramIndex = 0;

        var idProperty = PropertyDiscovery.GetProperty("Id", type);
        foreach (var property in properties)
        {
            if (property.Name == idProperty.Name) continue;
            var columnName = AttributeDiscovery.GetColumnName(property);
            var paramName = $"@p{paramIndex++}";
            columns.Add($"{columnName} = {paramName}");

            var columnValue = ValueDiscovery.GetValue(instance, property);
            var sqlParameter = new SqliteParameter(paramName, columnValue);
            command.Parameters.Add(sqlParameter);
        }

        result.AppendLine(string.Join(",\r\n", columns));

        result.AppendLine("WHERE");

        var idValue = ValueDiscovery.GetValue(instance, idProperty);
        var idParamName = $"@p{paramIndex}";
        result.AppendLine($"Id = {idParamName}");

        var idSqlParameter = new SqliteParameter(idParamName, idValue);
        command.Parameters.Add(idSqlParameter);

        command.CommandText = result.ToString();

        return command;
    }
}