namespace GymTracker.Domain.Enums;

public enum TimeUnit
{
    Seconds,
    Minutes,
    Hours
}

public static class TimeUnitExtensions
{
    public static string ToFriendlyString(this TimeUnit timeUnit)
    {
        return timeUnit switch
        {
            TimeUnit.Seconds => "Seconds",
            TimeUnit.Minutes => "Minutes",
            TimeUnit.Hours => "Hours",
            _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null)
        };
    }
}
