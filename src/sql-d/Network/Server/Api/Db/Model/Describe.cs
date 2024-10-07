namespace SqlD.Network.Server.Api.Db.Model
{
	public class Describe
	{
		public string TableName { get; set; }
		public static readonly IReadOnlyList<string> DescribeColumns = new[] { "cid", "name", "type", "notnull", "dflt_value", "pk" }.ToList();
		public static readonly IReadOnlyList<string> DescribeMasterColumns = new[] { "type", "name", "tbl_name", "sql" }.ToList();

		public static int IndexOf(string columnName)
		{
			return DescribeColumns.ToList().IndexOf(columnName);
		}
	}
}