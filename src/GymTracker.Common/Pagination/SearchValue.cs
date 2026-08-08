namespace GymTracker.Common.Pagination;

public struct SearchValue : IEquatable<SearchValue>
{
    public SearchValue(string? value)
    {
        Value = value?.Trim() ?? string.Empty;
    }

    public string Value { get; }

    public bool Equals(SearchValue other)
    {
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj) => obj is SearchValue other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);

    // Operators
    public static bool operator ==(SearchValue left, SearchValue right) => left.Equals(right);
    public static bool operator !=(SearchValue left, SearchValue right) => !left.Equals(right);

    public static implicit operator SearchValue(string value) => new SearchValue(value);
    public static explicit operator string(SearchValue searchValue) => searchValue.Value;

    public override string ToString()
    {
        return Value;
    }
}
