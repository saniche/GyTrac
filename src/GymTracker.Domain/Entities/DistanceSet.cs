using GymTracker.Domain.ValueObjects;

namespace GymTracker.Domain.Entities;

public sealed class DistanceSet : Set
{
    public Distance Distance { get; private set; } = null!;

    private DistanceSet() { }

    internal DistanceSet(Guid id, Guid exerciseLogId, Distance distance, int order, bool isWarmup = false, string? notes = null)
        : base(id, exerciseLogId, order, isWarmup, notes)
    {
        Distance = distance;
    }
}
