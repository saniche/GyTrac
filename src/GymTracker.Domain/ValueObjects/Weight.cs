using GymTracker.Domain.Enums;

namespace GymTracker.Domain.ValueObjects;

public sealed class Weight
{
    public decimal Value { get; private set; }
    public UnitOfMeasurement Unit { get; private set; }


    private Weight() { }

    public Weight(decimal value, UnitOfMeasurement unit)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Weight must be greater than zero.");
        }

        Value = value;
        Unit = unit;
    }

    public override string ToString()
    {
        return $"{Value} {Unit.ToFriendlyString()}";
    }
}