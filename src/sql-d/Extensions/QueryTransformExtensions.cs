using System.Reflection;
using SqlD.Attributes;
using SqlD.Extensions.Discovery;
using SqlD.Network.Server.Api.Db.Model;

namespace SqlD.Extensions
{
	internal static class QueryTransformExtensions
	{
		internal static List<T> TransformTo<T>(this QueryResponse response) where T : new()
		{
			var results = new List<T>();

			var type = typeof(T);
			var properties = PropertyDiscovery.GetProperties(type);

			foreach (var row in response.Rows)
			{
				var result = new T();

				foreach (var prop in response.Query.Properties)
				{
					var ordinal = response.Query.Properties.IndexOf(prop);
					var property = properties.FirstOrDefault(x => x.Name.ToUpper() == prop.ToUpper());

					var columnType = AttributeDiscovery.GetColumnType(property);
					var columnName = AttributeDiscovery.GetColumnName(property);

					if (columnName.ToUpper() == "ID")
					{
						ReadInteger(property, row[ordinal], result);
						continue;
					}

					switch (columnType)
					{
						case SqlLiteType.Integer:
						{
							ReadInteger(property, row[ordinal], result);
							break;
						}
						case SqlLiteType.Real:
						{
							ReadReal(property, row[ordinal], result);
							break;
						}
						case SqlLiteType.Text:
						{
							ReadString(property, row[ordinal], result);
							break;
						}
						case SqlLiteType.Blob:
						{
							throw new NotSupportedException("Cannot do blobs yet...");
						}
					}
				}

				results.Add(result);
			}

			return results;
		}

		private static void ReadString<T>(PropertyInfo property, object value, T result) where T : new()
		{
			if (property.PropertyType == typeof(string))
				property.SetValue(result, value);
			if (property.PropertyType == typeof(DateTime))
				if (value != null)
					property.SetValue(result, DateTime.Parse(value.ToString()));
		}

		private static void ReadReal<T>(PropertyInfo property, object value, T result) where T : new()
		{
			if (property.PropertyType == typeof(double))
			{
				var convertedValue = Convert.ToDouble(value);
				property.SetValue(result, convertedValue);
			}
			if (property.PropertyType == typeof(decimal))
			{
				var convertedValue = Convert.ToDecimal(value);
				property.SetValue(result, convertedValue);
			}
			if (property.PropertyType == typeof(float))
				property.SetValue(result, value);
		}

		private static void ReadInteger<T>(PropertyInfo property, object value, T result) where T : new()
		{
			if (property.PropertyType == typeof(short))
			{
				var convertedValue = Convert.ToByte(value);
				property.SetValue(result, convertedValue);
			}
			if (property.PropertyType == typeof(int))
			{
				var convertedValue = Convert.ToInt32(value);
				property.SetValue(result, convertedValue);
			}
			if (property.PropertyType == typeof(long))
			{
				var convertedValue = Convert.ToInt64(value);
				property.SetValue(result, convertedValue);
			}
		}
	}
}