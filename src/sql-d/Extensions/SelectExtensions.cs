using System.Text;
using SqlD.Extensions.Discovery;

namespace SqlD.Extensions
{
	internal static class SelectExtensions
	{
		internal static string GetCount(this Type type)
		{
			var result = new StringBuilder();

			result.AppendLine("SELECT COUNT(*)");
			result.AppendLine("FROM");

			var tableName = AttributeDiscovery.GetTableName(type);
			result.AppendLine(tableName);

			return result.ToString();
		}

		internal static string GetSelect(this Type type, string whereOrLimit = null)
		{
			var result = new StringBuilder();

			result.AppendLine("SELECT");

			var columns = new List<string>();

			var properties = PropertyDiscovery.GetProperties(type);

			foreach (var property in properties)
			{
				var columnName = AttributeDiscovery.GetColumnName(property);
				columns.Add(columnName);
			}

			result.AppendLine(string.Join(",\r\n", columns));
			result.AppendLine("FROM");

			var tableName = AttributeDiscovery.GetTableName(type);
			result.AppendLine(tableName);

			if (!string.IsNullOrEmpty(whereOrLimit))
			{
				result.AppendLine(whereOrLimit);
			}

			return result.ToString();
		}

		internal static List<string> GetColumns(this Type type)
		{
			var columns = new List<string>();
			var properties = PropertyDiscovery.GetProperties(type);
			foreach (var property in properties)
			{
				var columnName = AttributeDiscovery.GetColumnName(property);
				columns.Add(columnName);
			}
			return columns;
		}

		internal static List<string> GetPropertyInfoNames(this Type type)
		{
			var props = new List<string>();
			var properties = PropertyDiscovery.GetProperties(type);
			foreach (var property in properties)
			{
				props.Add(property.Name);
			}
			return props;
		}
	}
}