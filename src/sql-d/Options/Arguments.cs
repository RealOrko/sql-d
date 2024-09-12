namespace SqlD.Options;

[Command(Command = "SqlD.Start")]
public class Arguments
{
    [Argument(ShortName = "-n", LongName = "-name", Help = "Name of the Service being launched eg. -n \"Shiny-New-Service\"")]
    public string Name { get; set; }

    [Argument(ShortName = "-s", LongName = "-server", Help = "Service to launch eg. -s \"localhost:54321\"")]
    public string Service { get; set; }

    [Argument(ShortName = "-d", LongName = "-database", Help = "Database for services eg. -d \"localhost_54321.db\"")]
    public string Database { get; set; }

    [Argument(ShortName = "-pj", LongName = "-pragmajournalmode", Help = "For setting the pragma journal mode eg. -pj \"WAL\"")]
    public string PragmaJournalMode { get; set; }

    [Argument(ShortName = "-ps", LongName = "-pragmasynchronous", Help = "For setting the pragma synchronous eg. -ps \"OFF\"")]
    public string PragmaSynchronous { get; set; }

    [Argument(ShortName = "-pt", LongName = "-pragmatempstore", Help = "For setting the pragma temp store eg. -pt \"OFF\"")]
    public string PragmaTempStore { get; set; }

    [Argument(ShortName = "-pl", LongName = "-pragmalockingmode", Help = "For setting the pragma locking mode eg. -pl \"OFF\"")]
    public string PragmaLockingMode { get; set; }

    [Argument(ShortName = "-pc", LongName = "-pragmacountchanges", Help = "For setting the pragma count changes eg. -pc \"OFF\"")]
    public string PragmaCountChanges { get; set; }

    [Argument(ShortName = "-pps", LongName = "-pragmapagesize", Help = "For setting the pragma page size in bytes eg. -pps \"65536\"")]
    public string PragmaPageSize { get; set; }

    [Argument(ShortName = "-pcs", LongName = "-pragmacachesize", Help = "For setting the pragma cache size in bytes eg. -pcs \"10000\"")]
    public string PragmaCacheSize { get; set; }

    [Argument(ShortName = "-pqo", LongName = "-pragmaqueryonly", Help = "For setting the pragma query only eg. -pqo \"OFF\"")]
    public string PragmaQueryOnly { get; set; }

    [Argument(ShortName = "-r", LongName = "-registry", IsList = true, Help = "Registry for services eg. -r \"localhost:54300\"")]
    public List<string> Registries { get; set; }

    [Argument(ShortName = "-f", LongName = "-forward", IsList = true, Help = "Forward all writes to target eg. -f \"localhost:54321\"")]
    public List<string> Forwards { get; set; }

    [Argument(ShortName = "-t", LongName = "-tags", IsList = true, Help = "For tagging the service eg. -t \"development\"")]
    public List<string> Tags { get; set; }

    [Argument(ShortName = "-w", LongName = "-wait", Help = "Make a process wait, enter to quit eg. -w")]
    public bool Wait { get; set; }
}