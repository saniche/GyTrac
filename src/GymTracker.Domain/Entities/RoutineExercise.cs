namespace GymTracker.Domain.Entities;

/// <summary>
/// Represents the association between a workout routine and an exercise, including the order of the exercise within the routine.
/// </summary>
public class RoutineExercise
{
    public Guid RoutineId { get; private set; }
    public Guid ExerciseId { get; private set; }
    public int Order { get; private set; }

    public Routine Routine { get; private set; } = null!;
    public Exercise Exercise { get; private set; } = null!;

    private RoutineExercise() { }

    internal RoutineExercise(Guid routineId, Guid exerciseId, int order)
    {
        RoutineId = routineId;
        ExerciseId = exerciseId;
        Order = order;
    }
}
