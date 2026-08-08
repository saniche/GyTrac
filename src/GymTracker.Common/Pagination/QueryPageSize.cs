namespace GymTracker.Common.Pagination;

public struct QueryPageSize : IEquatable<QueryPageSize>
{
    public QueryPageSize(int value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentOutOfRangeException(nameof(value), "Page size must be between 0 and 100.");

        Value = value < 1 ? 10 : value;
    }

    public int Value { get; }

    public bool Equals(QueryPageSize other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object obj) => obj is QueryPageSize other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();

    // Operators
    public static bool operator ==(QueryPageSize left, QueryPageSize right) => left.Equals(right);
    public static bool operator !=(QueryPageSize left, QueryPageSize right) => !left.Equals(right);

    public static implicit operator QueryPageSize(int value) => new QueryPageSize(value);
    public static explicit operator int(QueryPageSize pageSize) => pageSize.Value;

    public override string ToString()
    {
        return Value.ToString();
    }
}
