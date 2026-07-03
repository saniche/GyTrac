namespace GymTracker.Domain.Enums;

public enum UnitOfMeasurement
{
    Kilograms,
    Pounds
}

public static class UnitOfMeasurementExtensions
{
    public static string ToFriendlyString(this UnitOfMeasurement unitOfMeasurement)
    {
        return unitOfMeasurement switch
        {
            UnitOfMeasurement.Kilograms => "Kilograms",
            UnitOfMeasurement.Pounds => "Pounds",
            _ => throw new ArgumentOutOfRangeException(nameof(unitOfMeasurement), unitOfMeasurement, null)
        };
    }
}