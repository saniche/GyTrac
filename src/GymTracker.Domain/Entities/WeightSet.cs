using GymTracker.Domain.ValueObjects;

namespace GymTracker.Domain.Entities;

public sealed class WeightSet : Set
{
    public Weight Weight { get; private set; } = null!;
    public int Reps { get; private set; }

    private WeightSet() { }

    internal WeightSet(Guid id, Guid exerciseLogId, Weight weight, int reps, int order, bool isWarmup = false, string? notes = null)
        : base(id, exerciseLogId, order, isWarmup, notes)
    {
        Weight = weight;
        Reps = reps;
    }
}
