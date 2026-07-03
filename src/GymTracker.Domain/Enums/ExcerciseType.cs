namespace GymTracker.Domain.Enums;

public enum ExerciseType
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

public static class ExerciseTypeExtensions
{
    public static string ToFriendlyString(this ExerciseType exerciseType)
    {
        return exerciseType switch
        {
            ExerciseType.Compound => "Compound",
            ExerciseType.Isolation => "Isolation",
            ExerciseType.Machine => "Machine",
            ExerciseType.Bodyweight => "Bodyweight",
            ExerciseType.Cardio => "Cardio",
            ExerciseType.Strength => "Strength",
            ExerciseType.Flexibility => "Flexibility",
            ExerciseType.Balance => "Balance",
            _ => throw new ArgumentOutOfRangeException(nameof(exerciseType), exerciseType, null)
        };
    }
}