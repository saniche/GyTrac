using Microsoft.EntityFrameworkCore;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure;

/// <summary>
/// Represents the database context for the GymTracker application, providing access to the application's entities.
/// </summary>
public class GyTracDbContext : DbContext
{
    public GyTracDbContext(DbContextOptions<GyTracDbContext> options) : base(options)
    {
    }

    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<WeightSet> WeightRepetitionSets => Set<WeightSet>();
    public DbSet<DistanceSet> DistanceSets => Set<DistanceSet>();
    public DbSet<DurationSet> DurationSets => Set<DurationSet>();

    public DbSet<ExerciseLog> ExerciseLogs => Set<ExerciseLog>();

    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();

    public DbSet<Routine> Routines => Set<Routine>();

    public DbSet<RoutineExercise> RoutineExercises => Set<RoutineExercise>();

    public DbSet<WorkoutProgram> WorkoutPrograms => Set<WorkoutProgram>();

    public DbSet<DistanceDurationSet> Sets => Set<DistanceDurationSet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GyTracDbContext).Assembly);
    }
}