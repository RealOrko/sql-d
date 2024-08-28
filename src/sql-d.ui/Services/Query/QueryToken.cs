using System.Collections.Generic;

namespace SqlD.UI.Services.Query;

public class QueryToken
{
	public static readonly QueryToken DESCRIBE = new QueryToken("?");
	public static readonly QueryToken SELECT = new QueryToken("select");
	public static readonly QueryToken INSERT = new QueryToken("insert");
	public static readonly QueryToken UPDATE = new QueryToken("update");
	public static readonly QueryToken DELETE = new QueryToken("delete");
	public static readonly QueryToken CREATE = new QueryToken("create");
	public static readonly QueryToken ALTER = new QueryToken("alter");
	public static readonly QueryToken DROP = new QueryToken("drop");

	private readonly string value;

	public QueryToken(string value)
	{
		this.value = value;
	}

	public string Value => value;

	protected bool Equals(QueryToken other)
	{
		return string.Equals(value, other.value);
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((QueryToken) obj);
	}

	public override int GetHashCode()
	{
		return (value != null ? value.GetHashCode() : 0);
	}

	public static bool operator ==(QueryToken left, QueryToken right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(QueryToken left, QueryToken right)
	{
		return !Equals(left, right);
	}

	private sealed class ValueEqualityComparer : IEqualityComparer<QueryToken>
	{
		public bool Equals(QueryToken x, QueryToken y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(x, null)) return false;
			if (ReferenceEquals(y, null)) return false;
			if (x.GetType() != y.GetType()) return false;
			return string.Equals(x.value, y.value);
		}

		public int GetHashCode(QueryToken obj)
		{
			return (obj.value != null ? obj.value.GetHashCode() : 0);
		}
	}

	public static IEqualityComparer<QueryToken> ValueComparer { get; } = new ValueEqualityComparer();

	public override string ToString()
	{
		return $"{nameof(value)}: {value}";
	}
}