namespace SqlD.Attributes;

public class SqlLiteTableAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}