using System.Reflection;
using SqlD.Attributes;

namespace SqlD.Extensions.Discovery
{
	internal static class ValueDiscovery
	{
		internal static object GetValue(object instance, PropertyInfo property)
		{
			var attribute = AttributeDiscovery.GetColumnAttribute(property);
			if (attribute == null)
			{
				var value1 = property.GetValue(instance);
				if (value1 == null) return "NULL";
				return value1.ToString();
			}

			if (attribute.Type == SqlLiteType.Text)
			{
				if (property.PropertyType == typeof(string))
				{
					var value2 = property.GetValue(instance);
					if (value2 == null) return "NULL";
					return $"'{value2.ToString().Replace("'", "''")}'";
				}
				if (property.PropertyType == typeof(DateTime))
				{
					var value3 = (DateTime) property.GetValue(instance);
					return value3.ToSqlLiteString();
				}
			}

			var value4 = property.GetValue(instance);
			if (value4 == null) return "NULL";
			return value4.ToString();
		}
	}
}