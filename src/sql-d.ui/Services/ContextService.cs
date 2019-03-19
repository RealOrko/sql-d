using Microsoft.AspNetCore.Http;

namespace SqlD.UI.Services
{
	public class ContextService
	{
		private readonly HttpContext context;

		public ContextService(HttpContext context)
		{
			this.context = context;
		}

		public bool Has(string cacheKey)
		{
			return Get<object>(cacheKey) != null;
		}

		public object Set(string cacheKey, object instance)
		{
			if (context != null)
			{
				context.Items[cacheKey] = instance;
			}

			return instance;
		}

		public object Get(string cacheKey)
		{
			return this.Get<object>(cacheKey);
		}

		public T Get<T>(string cacheKey)
		{
			if (context?.Items[cacheKey] != null)
			{
				return (T) context?.Items[cacheKey];
			}

			return default(T);
		}
	}
}