namespace SqlD.Attributes
{
	public class SqlLiteColumnAttribute : Attribute
	{
		public SqlLiteColumnAttribute(string name, SqlLiteType type, bool nullable)
		{
			Name = name;
			Type = type;
			Nullable = nullable;
		}

		public string Name { get; set; }
		public bool Nullable { get; set; }
		public SqlLiteType Type { get; set; }
	}
}