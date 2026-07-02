using GymTracker.Domain.ValueObjects;

namespace GymTracker.Domain.Entities;

public sealed class DurationSet : Set
{
    public Duration Duration { get; private set; } = null!;

    private DurationSet() { }

    internal DurationSet(Guid id, Guid exerciseLogId, Duration duration, int order, bool isWarmup = false, string? notes = null)
        : base(id, exerciseLogId, order, isWarmup, notes)
    {
        Duration = duration;
    }
}
