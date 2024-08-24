namespace SqlD.Attributes
{
	public class SqlLiteIndexAttribute : Attribute
	{
		public SqlLiteIndexAttribute(string indexName, SqlLiteIndexType indexType, params string[] additionalColumns)
		{
			IndexName = indexName;
			IndexType = indexType;
			AdditionalColumns = additionalColumns;
		}

		public string IndexName { get; set; }
		public SqlLiteIndexType IndexType { get; set; }
		public string[] AdditionalColumns { get; set; }
	}
}