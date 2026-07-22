using GymTracker.Domain.ValueObjects;

namespace GymTracker.Domain.Entities;

/// <summary>
/// Represents a set of an exercise that includes both distance and duration.
/// </summary>
public sealed class DistanceDurationSet : Set
{
    public Distance Distance { get; private set; } = null!;
    public Duration Duration { get; private set; } = null!;

    private DistanceDurationSet() { }

    internal DistanceDurationSet(Guid exerciseLogId, Distance distance, Duration duration, int order, bool isWarmup = false, string? notes = null)
        : base(exerciseLogId, order, isWarmup, notes)
    {
        Distance = distance;
        Duration = duration;
    }
}
