namespace SqlD.Options
{
    public class ArgumentsParser
	{
		public static T Parse<T>(string[] args) where T:new()
		{
			var result = new T();
			foreach (var propertyInfo in result.GetType().GetProperties())
			{
				var attribute = propertyInfo.GetCustomAttributes(typeof(ArgumentAttribute), inherit: false).Cast<ArgumentAttribute>().FirstOrDefault();
				if (attribute != null)
				{
					if (attribute.IsList && propertyInfo.GetValue(result) == null)
					{
						propertyInfo.SetValue(result, new List<string>());
					}

					foreach (var arg in args)
					{
						if (arg.Equals(attribute.ShortName) || arg.Equals(attribute.LongName))
						{
							if (propertyInfo.PropertyType == typeof(bool))
							{
								propertyInfo.SetValue(result, true);
								continue;
							}

							if (!attribute.IsList)
							{
								var value = FindValues(args, attribute).FirstOrDefault();
								propertyInfo.SetValue(result, value);
								continue;
							}

							if (attribute.IsList)
							{
								var values = FindValues(args, attribute);
								var arguments = (List<string>)propertyInfo.GetValue(result);
								if (arguments.Any()) continue;
								arguments.AddRange(values);
							}
						}
					}
				}
			}

			return result;
		}

		private static IEnumerable<string> FindValues(string[] args, ArgumentAttribute attribute)
		{
			var results = new List<string>();
			var currentArgIndex = 0;
			foreach (var arg in args)
			{
				if (arg == attribute.ShortName || arg == attribute.LongName)
				{
					if (currentArgIndex + 1 < args.Length)
					{
						results.Add(args[currentArgIndex + 1]);
					}
				}

				currentArgIndex++;
			}

			return results;
		}
	}
}