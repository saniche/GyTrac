namespace GymTracker.Common.Pagination;

public struct QueryPage : IEquatable<QueryPage>
{
    public QueryPage(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Page must be greater than or equal to 0.");

        Value = value < 1 ? 1 : value;
    }

    public int Value { get; }

    public bool Equals(QueryPage other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object obj) => obj is QueryPage other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();

    // Operators
    public static bool operator ==(QueryPage left, QueryPage right) => left.Equals(right);
    public static bool operator !=(QueryPage left, QueryPage right) => !left.Equals(right);

    public static implicit operator QueryPage(int value) => new QueryPage(value);
    public static explicit operator int(QueryPage page) => page.Value;

    public override string ToString()
    {
        return Value.ToString();
    }
}
