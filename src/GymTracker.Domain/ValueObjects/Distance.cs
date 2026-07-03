using GymTracker.Domain.Enums;

namespace GymTracker.Domain.ValueObjects;

public class Distance
{
    public decimal Value { get; private set; }
    public DistanceUnit Unit { get; private set; }

    private Distance() { }

    public Distance(decimal value, DistanceUnit unit)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Distance must be greater than zero.");
        }

        Value = value;
        Unit = unit;
    }

    public override string ToString()
    {
        return $"{Value} {Unit}";
    }
}