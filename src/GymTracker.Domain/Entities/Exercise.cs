using GymTracker.Domain.Enums;

namespace GymTracker.Domain.Entities;

/// <summary>
/// Represents an exercise that can be performed in a workout.
/// </summary>
public class Exercise
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public MuscleGroup PrimaryMuscleGroup { get; private set; }
    public ExerciseType Type { get; private set; }

    private Exercise() { }

    public Exercise(string name, MuscleGroup primaryMuscleGroup, ExerciseType type, string? description = null)
    {
        Id = Guid.Empty;
        Name = name;
        PrimaryMuscleGroup = primaryMuscleGroup;
        Type = type;
        Description = description;
    }
}
