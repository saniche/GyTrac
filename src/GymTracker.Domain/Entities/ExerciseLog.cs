using GymTracker.Domain.ValueObjects;

namespace GymTracker.Domain.Entities;

/// <summary>
/// Represents a log of an exercise performed during a workout session.
/// </summary>
public class ExerciseLog
{
    public Guid Id { get; private set; }
    public Guid WorkoutSessionId { get; private set; }
    public Guid ExerciseId { get; private set; }
    public int Order { get; private set; }

    public WorkoutSession WorkoutSession { get; private set; } = null!;
    public Exercise Exercise { get; private set; } = null!;

    private readonly List<Set> _sets = [];
    public IReadOnlyCollection<Set> Sets => _sets.AsReadOnly();

    private ExerciseLog() { }

    internal ExerciseLog(Guid workoutSessionId, Guid exerciseId, int order)
    {
        Id = Guid.Empty;
        WorkoutSessionId = workoutSessionId;
        ExerciseId = exerciseId;
        Order = order;
    }

    public void AddWeightSet(Weight weight, int reps, bool isWarmup = false, string? notes = null)
    {
        _sets.Add(new WeightSet(Id, weight, reps, _sets.Count + 1, isWarmup, notes));
    }

    public void AddDistanceSet(Distance distance, bool isWarmup = false, string? notes = null)
    {
        _sets.Add(new DistanceSet(Id, distance, _sets.Count + 1, isWarmup, notes));
    }

    public void AddDurationSet(Duration duration, bool isWarmup = false, string? notes = null)
    {
        _sets.Add(new DurationSet(Id, duration, _sets.Count + 1, isWarmup, notes));
    }

    public void AddDistanceDurationSet(Distance distance, Duration duration, bool isWarmup = false, string? notes = null)
    {
        _sets.Add(new DistanceDurationSet(Id, distance, duration, _sets.Count + 1, isWarmup, notes));
    }
}
