using System.Collections.Concurrent;
using System.Reflection;
using SqlD.Attributes;

namespace SqlD.Extensions.Discovery;

internal static class AttributeDiscovery
{
    private static readonly ConcurrentDictionary<string, SqlLiteTableAttribute> TableAttributes = new();

    private static readonly ConcurrentDictionary<string, SqlLiteColumnAttribute> ColumnAttributes = new();

    private static readonly ConcurrentDictionary<string, SqlLiteIndexAttribute> IndexAttributes = new();

    internal static string GetTableName(Type type)
    {
        CacheAttributes(type);

        SqlLiteTableAttribute tableAttribute = null;
        if (TableAttributes.TryGetValue(type.Name, out tableAttribute))
            return tableAttribute.Name;

        return type.Name;
    }

    internal static SqlLiteColumnAttribute GetColumnAttribute(PropertyInfo property)
    {
        CacheAttributes(property.DeclaringType);

        SqlLiteColumnAttribute columnAttribute = null;
        if (ColumnAttributes.TryGetValue($"{property.DeclaringType.Name}-{property.Name}", out columnAttribute))
            return columnAttribute;

        return null;
    }

    internal static string GetColumnName(PropertyInfo property)
    {
        CacheAttributes(property.DeclaringType);

        SqlLiteColumnAttribute columnAttribute = null;
        if (ColumnAttributes.TryGetValue($"{property.DeclaringType.Name}-{property.Name}", out columnAttribute))
            return columnAttribute.Name;

        if (property.Name.ToUpper() == "ID")
            return "ID";

        return property.Name;
    }

    internal static bool ShouldIgnoreColumn(PropertyInfo property)
    {
        return property.GetCustomAttributes<SqlLiteIgnoreAttribute>().Any();
    }

    internal static SqlLiteType GetColumnType(PropertyInfo property)
    {
        CacheAttributes(property.DeclaringType);

        SqlLiteColumnAttribute columnAttribute = null;
        if (ColumnAttributes.TryGetValue($"{property.DeclaringType.Name}-{property.Name}", out columnAttribute))
            return columnAttribute.Type;

        return SqlLiteType.Text;
    }

    internal static string GetColumnNullable(PropertyInfo property)
    {
        CacheAttributes(property.DeclaringType);

        SqlLiteColumnAttribute columnAttribute = null;
        if (ColumnAttributes.TryGetValue($"{property.DeclaringType.Name}-{property.Name}", out columnAttribute))
            if (columnAttribute.Nullable)
                return "NULL";

        return "NOT NULL";
    }

    internal static string GetIndexName(PropertyInfo property)
    {
        CacheAttributes(property.DeclaringType);

        SqlLiteIndexAttribute indexAttribute = null;
        if (IndexAttributes.TryGetValue($"{property.DeclaringType.Name}-{property.Name}", out indexAttribute)) return indexAttribute.IndexName;

        return null;
    }

    internal static string GetIndexUnique(PropertyInfo property)
    {
        CacheAttributes(property.DeclaringType);

        SqlLiteIndexAttribute indexAttribute = null;
        if (IndexAttributes.TryGetValue($"{property.DeclaringType.Name}-{property.Name}", out indexAttribute)) return indexAttribute.IndexType == SqlLiteIndexType.Unique ? "UNIQUE" : "";

        return string.Empty;
    }

    internal static string GetIndexAdditionalColumns(PropertyInfo property)
    {
        CacheAttributes(property.DeclaringType);

        SqlLiteIndexAttribute indexAttribute = null;
        if (IndexAttributes.TryGetValue($"{property.DeclaringType.Name}-{property.Name}", out indexAttribute))
        {
            if (indexAttribute.AdditionalColumns != null)
                return string.Join(",", indexAttribute.AdditionalColumns);
            return string.Empty;
        }

        return string.Empty;
    }

    private static void CacheAttributes(Type type)
    {
        if (TableAttributes.ContainsKey(type.Name))
            return;

        CacheTableAttribute(type);
        CacheIndexAttributes(type);
        CacheColumnAttributes(type);
    }

    private static void CacheTableAttribute(Type type)
    {
        var attributes = type.GetCustomAttributes(typeof(SqlLiteTableAttribute));
        if (attributes.Any())
            TableAttributes[type.Name] =
                attributes.Cast<SqlLiteTableAttribute>().First();
    }

    private static void CacheIndexAttributes(Type type)
    {
        foreach (var property in PropertyDiscovery.GetProperties(type))
        {
            var attributes = property.GetCustomAttributes(typeof(SqlLiteIndexAttribute));
            if (attributes.Any())
                IndexAttributes[$"{property.DeclaringType.Name}-{property.Name}"] =
                    attributes.Cast<SqlLiteIndexAttribute>().First();
        }
    }

    private static void CacheColumnAttributes(Type type)
    {
        foreach (var property in PropertyDiscovery.GetProperties(type))
        {
            var attributes = property.GetCustomAttributes(typeof(SqlLiteColumnAttribute));
            if (attributes.Any())
                ColumnAttributes[$"{property.DeclaringType.Name}-{property.Name}"] =
                    attributes.Cast<SqlLiteColumnAttribute>().First();
        }
    }
}