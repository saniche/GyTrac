using GymTracker.Domain.ValueObjects;

namespace GymTracker.Domain.Entities;

public sealed class DistanceDurationSet : Set
{
    public Distance Distance { get; private set; } = null!;
    public Duration Duration { get; private set; } = null!;

    private DistanceDurationSet() { }

    internal DistanceDurationSet(Guid id, Guid exerciseLogId, Distance distance, Duration duration, int order, bool isWarmup = false, string? notes = null)
        : base(id, exerciseLogId, order, isWarmup, notes)
    {
        Distance = distance;
        Duration = duration;
    }
}
