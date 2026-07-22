using GymTracker.Domain.ValueObjects;

namespace GymTracker.Domain.Entities;

/// <summary>
/// Represents a set of an exercise that includes weight and repetitions.
/// </summary>
public sealed class WeightSet : Set
{
    public Weight Weight { get; private set; } = null!;
    public int Reps { get; private set; }

    private WeightSet() { }

    internal WeightSet(Guid exerciseLogId, Weight weight, int reps, int order, bool isWarmup = false, string? notes = null)
        : base(exerciseLogId, order, isWarmup, notes)
    {
        Weight = weight;
        Reps = reps;
    }
}
