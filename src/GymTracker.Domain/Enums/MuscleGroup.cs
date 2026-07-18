namespace GymTracker.Domain.Enums;

public enum MuscleGroup
{
    None,
    Chest,
    Back,
    Shoulders,
    Biceps,
    Triceps,
    Legs,
    Core,
    Calves,
    Glutes,
    FullBody,
}

public static class MuscleGroupExtensions
{
    public static string ToFriendlyString(this MuscleGroup muscleGroup)
    {
        return muscleGroup switch
        {
            MuscleGroup.None => "None",
            MuscleGroup.Chest => "Chest",
            MuscleGroup.Back => "Back",
            MuscleGroup.Shoulders => "Shoulders",
            MuscleGroup.Biceps => "Biceps",
            MuscleGroup.Triceps => "Triceps",
            MuscleGroup.Legs => "Legs",
            MuscleGroup.Core => "Core",
            MuscleGroup.Calves => "Calves",
            MuscleGroup.Glutes => "Glutes",
            MuscleGroup.FullBody => "Full Body",
            _ => throw new ArgumentOutOfRangeException(nameof(muscleGroup), muscleGroup, null)
        };
    }
}
