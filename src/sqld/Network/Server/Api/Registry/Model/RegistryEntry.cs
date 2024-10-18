using SqlD.Attributes;

namespace SqlD.Network.Server.Api.Registry.Model;

[SqlLiteTable("Registry")]
public class RegistryEntry
{
    public long Id { get; set; }

    [SqlLiteColumn("Name", SqlLiteType.Text, false)]
    public string Name { get; set; }

    [SqlLiteColumn("Database", SqlLiteType.Text, false)]
    public string Database { get; set; }

    [SqlLiteColumn("Uri", SqlLiteType.Text, false)]
    public string Uri { get; set; }

    [SqlLiteColumn("Tags", SqlLiteType.Text, false)]
    public string Tags { get; set; }

    [SqlLiteColumn("LastUpdated", SqlLiteType.Text, false)]
    public DateTime LastUpdated { get; set; }

    [SqlLiteColumn("AuthorityUri", SqlLiteType.Text, false)]
    public string AuthorityUri { get; set; }

    [SqlLiteIgnore] 
    public string[] TagsAsArray => Tags.Split(',');

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Uri)}: {Uri}, {nameof(Tags)}: {Tags}";
    }
}