namespace SqlD.Options
{
    public class ArgumentAttribute : Attribute
	{
		public bool IsList { get; set; } = false;
		public string ShortName { get; set; }
		public string LongName { get; set; }
		public string Help { get; set; }
	}
}