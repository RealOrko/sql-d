using System.Collections.Concurrent;
using System.Reflection;

namespace SqlD.Extensions.Discovery
{
	internal static class PropertyDiscovery
	{
		private static readonly ConcurrentDictionary<string, PropertyInfo> Properties =
			new ConcurrentDictionary<string, PropertyInfo>();

		private static readonly ConcurrentDictionary<string, PropertyInfo[]> TypeProperties =
			new ConcurrentDictionary<string, PropertyInfo[]>();

		public static PropertyInfo GetProperty(string name, Type type)
		{
			PropertyInfo property = null;
			var propertyKey = $"{type.Name}-{name}";
			if (Properties.TryGetValue(propertyKey, out property))
				return property;

			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (var prop in properties)
			{
				if (prop.Name == name) property = prop;
				Properties[$"{type.Name}-{prop.Name}"] = prop;
			}

			return property;
		}

		public static PropertyInfo[] GetProperties(Type type)
		{
			PropertyInfo[] properties = null;
			if (TypeProperties.TryGetValue(type.Name, out properties))
				return properties;

			properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var filteredProperties = properties.Where(x => !AttributeDiscovery.ShouldIgnoreColumn(x)).ToArray();
			TypeProperties[type.Name] = filteredProperties;

			return filteredProperties;
		} 
	}
}