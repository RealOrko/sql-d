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

    [SqlLiteIgnore] public ServiceStateType State { get; set; }

    [SqlLiteIgnore] public string[] TagsAsArray => Tags.Split(',');

    public EndPoint ToEndPoint()
    {
        var uri = new Uri(Uri);
        return new EndPoint(uri.Host, uri.Port);
    }

    protected bool Equals(RegistryEntry other)
    {
        return string.Equals(Uri, other.Uri) && string.Equals(AuthorityUri, other.AuthorityUri);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((RegistryEntry)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Uri != null ? Uri.GetHashCode() : 0) * 397) ^ (AuthorityUri != null ? AuthorityUri.GetHashCode() : 0);
        }
    }

    public static bool operator ==(RegistryEntry left, RegistryEntry right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(RegistryEntry left, RegistryEntry right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Uri)}: {Uri}, {nameof(Tags)}: {Tags}";
    }
}