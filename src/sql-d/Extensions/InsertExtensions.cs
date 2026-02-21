using Microsoft.Data.Sqlite;
using System.Text;
using SqlD.Extensions.Discovery;

namespace SqlD.Extensions;

internal static class InsertExtensions
{
    [Obsolete("This does not use prepared statements, please do not use")]
    internal static string GetInsert(this object instance, bool withIdentity = false)
    {
        var result = new StringBuilder();

        var type = instance.GetType();

        var tableName = AttributeDiscovery.GetTableName(type);
        result.AppendLine($"INSERT INTO {tableName}");
        result.AppendLine("(");

        var columns = new List<string>();

        var properties = PropertyDiscovery.GetProperties(type);

        var idProperty = PropertyDiscovery.GetProperty("Id", type);
        foreach (var property in properties)
        {
            if (property.Name == idProperty.Name && !withIdentity) continue;
            var columnName = AttributeDiscovery.GetColumnName(property);
            columns.Add(columnName);
        }

        result.AppendLine(string.Join(",\r\n", columns));
        result.AppendLine(")");
        result.AppendLine("VALUES");
        result.AppendLine("(");

        var values = new List<string>();

        foreach (var property in properties)
        {
            if (property.Name == idProperty.Name && !withIdentity) continue;
            if (property.Name == idProperty.Name)
            {
                var value = (string)ValueDiscovery.GetValue(instance, property);
                values.Add(value == "0" ? "NULL" : value);
            }
            else
            {
                var value = (string)ValueDiscovery.GetValue(instance, property);
                values.Add(value);
            }
        }

        result.AppendLine(string.Join(",\r\n", values));
        result.AppendLine(");");
        result.AppendLine("SELECT last_insert_rowid();");
        return result.ToString();
    }

    internal static SqliteCommand GetInsertPrepared(this object instance, SqliteConnection connection, bool withIdentity = false)
    {
        var command = connection.CreateCommand();

        var result = new StringBuilder();

        var type = instance.GetType();

        var tableName = AttributeDiscovery.GetTableName(type);
        result.AppendLine($"INSERT INTO {tableName}");
        result.AppendLine("(");

        var columns = new List<string>();

        var properties = PropertyDiscovery.GetProperties(type);

        var idProperty = PropertyDiscovery.GetProperty("Id", type);
        foreach (var property in properties)
        {
            if (property.Name == idProperty.Name && !withIdentity) continue;
            var columnName = AttributeDiscovery.GetColumnName(property);
            columns.Add(columnName);
        }

        result.AppendLine(string.Join(",\r\n", columns));
        result.AppendLine(")");
        result.AppendLine("VALUES");
        result.AppendLine("(");

        var values = new List<string>();
        var paramIndex = 0;

        foreach (var property in properties)
        {
            if (property.Name == idProperty.Name && !withIdentity) continue;
            var paramName = $"@p{paramIndex++}";
            values.Add(paramName);
            var instanceValue = property.GetValue(instance);

            var sqlParameter = new SqliteParameter(paramName, instanceValue);
            command.Parameters.Add(sqlParameter);
        }

        result.AppendLine(string.Join(",\r\n", values));
        result.AppendLine(");");
        result.AppendLine("SELECT last_insert_rowid();");

        command.CommandText = result.ToString();
        return command;
    }
}