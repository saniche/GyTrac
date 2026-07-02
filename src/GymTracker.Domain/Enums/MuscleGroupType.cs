namespace GymTracker.Domain.Enums;

public enum MuscleGroupType
{
    UpperBody,
    LowerBody,
    FullBody
}

public static class MuscleGroupTypeExtensions
{
    public static string ToFriendlyString(this MuscleGroupType muscleGroupType)
    {
        return muscleGroupType switch
        {
            MuscleGroupType.UpperBody => "Upper Body",
            MuscleGroupType.LowerBody => "Lower Body",
            MuscleGroupType.FullBody => "Full Body",
            _ => throw new ArgumentOutOfRangeException(nameof(muscleGroupType), muscleGroupType, null)
        };
    }
}
