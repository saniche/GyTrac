using GymTracker.Domain.ValueObjects;

namespace GymTracker.Domain.Entities;

/// <summary>
/// Represents a set of an exercise that includes distance.
/// </summary>
public sealed class DistanceSet : Set
{
    public Distance Distance { get; private set; } = null!;

    private DistanceSet() { }

    internal DistanceSet(Guid exerciseLogId, Distance distance, int order, bool isWarmup = false, string? notes = null)
        : base(exerciseLogId, order, isWarmup, notes)
    {
        Distance = distance;
    }
}
