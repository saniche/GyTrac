namespace GymTracker.Domain.Entities;

public class WorkoutSession
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public Guid? RoutineId { get; private set; }
    public string? Notes { get; private set; }

    private readonly List<ExerciseLog> _exerciseLogs = [];
    public IReadOnlyCollection<ExerciseLog> ExerciseLogs => _exerciseLogs.AsReadOnly();

    private WorkoutSession() { }

    public WorkoutSession(Guid id, Guid userId, DateTimeOffset startedAt, Guid? routineId = null, string? notes = null)
    {
        Id = id;
        UserId = userId;
        StartedAt = startedAt;
        RoutineId = routineId;
        Notes = notes;
    }

    public void Complete(DateTimeOffset completedAt) => CompletedAt = completedAt;

    public ExerciseLog AddExerciseLog(Guid exerciseId, int order)
    {
        var log = new ExerciseLog(Guid.NewGuid(), Id, exerciseId, order);
        _exerciseLogs.Add(log);
        return log;
    }
}
