using GymTracker.Domain.Enums;

namespace GymTracker.Domain.ValueObjects;

public class Duration
{
    public int Value { get; private set; }
    public TimeUnit Unit { get; private set; }

    private Duration() { }

    public Duration(int value, TimeUnit unit)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Duration must be greater than zero.");
        }

        Value = value;
        Unit = unit;
    }

    public override string ToString()
    {
        return $"{Value} {Unit.ToFriendlyString()}";
    }
}

