namespace SqlD.UI.Models.Discovery
{
	public class LinkViewModel
	{
		public string Rel { get; }
		public string Authority { get; }
		public string Host { get; }
		public string Uri { get; }
		public string Tags { get; set; }

		public LinkViewModel(string uri, string rel, string authority, string host, string tags)
		{
			Rel = rel;
			Authority = authority;
			Host = host;
			Uri = uri;
			Tags = tags ?? string.Empty;
		}

		public override string ToString()
		{
			return $"{nameof(Rel)}: {Rel}, {nameof(Authority)}: {Authority}, {nameof(Uri)}: {Uri}, {nameof(Host)}: {Host}, {nameof(Tags)}: {Tags}";
		}
	}
}