using GymTracker.Domain.ValueObjects;

namespace GymTracker.Domain.Entities;

/// <summary>
/// Represents a set of an exercise that includes duration.
/// </summary>
public sealed class DurationSet : Set
{
    public Duration Duration { get; private set; } = null!;

    private DurationSet() { }

    internal DurationSet(Guid exerciseLogId, Duration duration, int order, bool isWarmup = false, string? notes = null)
        : base(exerciseLogId, order, isWarmup, notes)
    {
        Duration = duration;
    }
}
