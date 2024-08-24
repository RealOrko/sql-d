namespace SqlD.Attributes
{
	public class SqlLiteTableAttribute : Attribute
	{
		public SqlLiteTableAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}