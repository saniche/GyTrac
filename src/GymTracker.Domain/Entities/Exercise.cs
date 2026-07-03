using GymTracker.Domain.Enums;

namespace GymTracker.Domain.Entities;

public class Exercise
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public MuscleGroup PrimaryMuscleGroup { get; private set; }
    public ExerciseType Type { get; private set; }

    private Exercise() { }

    public Exercise(Guid id, string name, MuscleGroup primaryMuscleGroup, ExerciseType type, string? description = null)
    {
        Id = id;
        Name = name;
        PrimaryMuscleGroup = primaryMuscleGroup;
        Type = type;
        Description = description;
    }
}
