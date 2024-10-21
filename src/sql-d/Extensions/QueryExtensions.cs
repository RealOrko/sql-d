using System.Data.Common;
using System.Reflection;
using SqlD.Attributes;
using SqlD.Extensions.Discovery;

namespace SqlD.Extensions;

internal static class QueryExtensions
{
    internal static List<T> Query<T>(this DbConnection dbConnection, string where = null) where T : new()
    {
        var type = typeof(T);
        var results = new List<T>();
        var properties = PropertyDiscovery.GetProperties(type);
        var query = type.GetSelect(where);

        using (var dbReader = dbConnection.ExecuteQuery(query))
        {
            var reader = dbReader.Reader;
            while (reader.Read())
            {
                var result = new T();

                foreach (var property in properties)
                {
                    int ordinal;
                    var columnType = AttributeDiscovery.GetColumnType(property);
                    var columnName = AttributeDiscovery.GetColumnName(property);

                    if (columnName.ToUpper() == "ID")
                    {
                        ordinal = reader.GetOrdinal(columnName.ToUpper());
                        ReadInteger(property, reader, ordinal, result);
                        continue;
                    }

                    ordinal = reader.GetOrdinal(columnName);

                    switch (columnType)
                    {
                        case SqlLiteType.Integer:
                        {
                            ReadInteger(property, reader, ordinal, result);
                            break;
                        }
                        case SqlLiteType.Real:
                        {
                            ReadReal(property, reader, ordinal, result);
                            break;
                        }
                        case SqlLiteType.Text:
                        {
                            ReadString(reader, ordinal, property, result);
                            break;
                        }
                        case SqlLiteType.Blob:
                        {
                            throw new NotSupportedException("Cannot do blobs yet...");
                        }
                    }
                }

                results.Add(result);
            }
        }

        return results;
    }

    internal static async Task<List<T>> QueryAsync<T>(this DbConnection dbConnection, string where = null) where T : new()
    {
        var type = typeof(T);
        var results = new List<T>();
        var properties = PropertyDiscovery.GetProperties(type);
        var query = type.GetSelect(where);

        using (var dbReader = await dbConnection.ExecuteQueryAsync(query))
        {
            var reader = dbReader.Reader;
            while (reader.Read())
            {
                var result = new T();

                foreach (var property in properties)
                {
                    int ordinal;
                    var columnType = AttributeDiscovery.GetColumnType(property);
                    var columnName = AttributeDiscovery.GetColumnName(property);

                    if (columnName.ToUpper() == "ID")
                    {
                        ordinal = reader.GetOrdinal(columnName.ToUpper());
                        ReadInteger(property, reader, ordinal, result);
                        continue;
                    }

                    ordinal = reader.GetOrdinal(columnName);

                    switch (columnType)
                    {
                        case SqlLiteType.Integer:
                        {
                            ReadInteger(property, reader, ordinal, result);
                            break;
                        }
                        case SqlLiteType.Real:
                        {
                            ReadReal(property, reader, ordinal, result);
                            break;
                        }
                        case SqlLiteType.Text:
                        {
                            ReadString(reader, ordinal, property, result);
                            break;
                        }
                        case SqlLiteType.Blob:
                        {
                            throw new NotSupportedException("Cannot do blobs yet...");
                        }
                    }
                }

                results.Add(result);
            }
        }

        return results;
    }

    private static void ReadString<T>(DbDataReader reader, int ordinal, PropertyInfo property, T result) where T : new()
    {
        if (property.PropertyType == typeof(string))
        {
            var value = reader.GetString(ordinal);
            property.SetValue(result, value.Replace("'", ""));
        }

        if (property.PropertyType == typeof(DateTime))
        {
            var value = reader.GetString(ordinal);
            if (value != null)
                property.SetValue(result, DateTime.Parse(value.Replace("'", "")));
        }
    }

    private static void ReadReal<T>(PropertyInfo property, DbDataReader reader, int ordinal, T result) where T : new()
    {
        if (property.PropertyType == typeof(double))
        {
            var value = reader.GetDouble(ordinal);
            property.SetValue(result, value);
        }

        if (property.PropertyType == typeof(decimal))
        {
            var value = reader.GetDecimal(ordinal);
            property.SetValue(result, value);
        }

        if (property.PropertyType == typeof(float))
        {
            var value = reader.GetFloat(ordinal);
            property.SetValue(result, value);
        }
    }

    private static void ReadInteger<T>(PropertyInfo property, DbDataReader reader, int ordinal, T result) where T : new()
    {
        if (property.PropertyType == typeof(short))
        {
            var value = reader.GetByte(ordinal);
            property.SetValue(result, value);
        }

        if (property.PropertyType == typeof(int))
        {
            var value = reader.GetInt32(ordinal);
            property.SetValue(result, value);
        }

        if (property.PropertyType == typeof(long))
        {
            var value = reader.GetInt64(ordinal);
            property.SetValue(result, value);
        }
    }
}