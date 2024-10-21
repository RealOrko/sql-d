namespace SqlD.Attributes;

public class SqlLiteColumnAttribute(string name, SqlLiteType type, bool nullable) : Attribute
{
    public string Name { get; } = name;
    public bool Nullable { get; } = nullable;
    public SqlLiteType Type { get; } = type;
}

