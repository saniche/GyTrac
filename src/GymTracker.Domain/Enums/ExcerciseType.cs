namespace GymTracker.Domain.Enums;

public enum ExcerciseType
{
    Compound,
    Isolation,
    Machine,
    Bodyweight,
    Cardio,
    Strength,
    Flexibility,
    Balance,
}

public static class ExcerciseTypeExtensions
{
    public static string ToFriendlyString(this ExcerciseType excerciseType)
    {
        return excerciseType switch
        {
            ExcerciseType.Compound => "Compound",
            ExcerciseType.Isolation => "Isolation",
            ExcerciseType.Machine => "Machine",
            ExcerciseType.Bodyweight => "Bodyweight",
            ExcerciseType.Cardio => "Cardio",
            ExcerciseType.Strength => "Strength",
            ExcerciseType.Flexibility => "Flexibility",
            ExcerciseType.Balance => "Balance",
            _ => throw new ArgumentOutOfRangeException(nameof(excerciseType), excerciseType, null)
        };
    }
}