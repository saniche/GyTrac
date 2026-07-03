namespace GymTracker.Domain.Entities;

public abstract class Set
{
    public Guid Id { get; private set; }
    public Guid ExerciseLogId { get; private set; }
    public int Order { get; private set; }
    public bool IsWarmup { get; private set; }
    public string? Notes { get; private set; }

    protected Set() { }

    protected Set(Guid id, Guid exerciseLogId, int order, bool isWarmup, string? notes)
    {
        Id = id;
        ExerciseLogId = exerciseLogId;
        Order = order;
        IsWarmup = isWarmup;
        Notes = notes;
    }
}
