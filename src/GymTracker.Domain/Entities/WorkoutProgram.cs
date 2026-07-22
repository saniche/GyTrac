namespace GymTracker.Domain.Entities;

/// <summary>
/// Represents a workout program that can contain multiple routines.
/// </summary>
public class WorkoutProgram
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private readonly List<ProgramRoutine> _routines = [];
    public IReadOnlyCollection<ProgramRoutine> Routines => _routines.AsReadOnly();

    private WorkoutProgram() { }

    public WorkoutProgram(Guid userId, string name, string? description = null)
    {
        Id = Guid.Empty;
        UserId = userId;
        Name = name;
        Description = description;
    }

    public void AddRoutine(Guid routineId, int order)
    {
        _routines.Add(new ProgramRoutine(Id, routineId, order));
    }
}

public class ProgramRoutine
{
    public Guid WorkoutProgramId { get; private set; }
    public Guid RoutineId { get; private set; }
    public int Order { get; private set; }

    public WorkoutProgram WorkoutProgram { get; private set; } = null!;
    public Routine Routine { get; private set; } = null!;

    private ProgramRoutine() { }

    internal ProgramRoutine(Guid workoutProgramId, Guid routineId, int order)
    {
        WorkoutProgramId = workoutProgramId;
        RoutineId = routineId;
        Order = order;
    }
}
