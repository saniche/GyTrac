namespace GymTracker.Domain.Entities;

/// <summary>
/// Represents a workout routine that a user can follow, consisting of a collection of exercises.
/// </summary>
public class Routine
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private readonly List<RoutineExercise> _exercises = [];
    public IReadOnlyCollection<RoutineExercise> Exercises => _exercises.AsReadOnly();

    private Routine() { }

    public Routine(Guid id, Guid userId, string name, string? description = null)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Description = description;
    }

    public void AddExercise(Guid exerciseId, int order)
    {
        _exercises.Add(new RoutineExercise(Id, exerciseId, order));
    }
}
