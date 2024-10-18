namespace SqlD.Attributes;

public class SqlLiteIndexAttribute(string indexName, SqlLiteIndexType indexType, params string[] additionalColumns) : Attribute
{
    public string IndexName { get; } = indexName;
    public SqlLiteIndexType IndexType { get; } = indexType;
    public string[] AdditionalColumns { get; } = additionalColumns;
}