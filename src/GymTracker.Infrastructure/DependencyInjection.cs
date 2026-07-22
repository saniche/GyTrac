
using GymTracker.Domain.Entities;
using GymTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.Infrastructure;


/// <summary>
/// Provides extension methods for configuring and initializing the infrastructure layer of the application.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GyTracDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GyTracDbContext>();

        if (!await dbContext.Exercises.AnyAsync())
        {
            // Chest, Back, Legs, Shoulders, Arms, Core
            var defaultExercises = new List<Exercise>
            {
                new("Bench Press", MuscleGroup.Chest, ExerciseType.Strength),
                new("Pull-Up", MuscleGroup.Back, ExerciseType.Strength),
                new("Squat", MuscleGroup.Legs, ExerciseType.Strength),
                new("Shoulder Press", MuscleGroup.Shoulders, ExerciseType.Strength),
                new("Bicep Curl", MuscleGroup.Biceps, ExerciseType.Strength),
                new("Tricep Extension", MuscleGroup.Triceps, ExerciseType.Strength),
                new("Plank", MuscleGroup.Core, ExerciseType.Strength),
                new("Running", MuscleGroup.Legs, ExerciseType.Cardio),
                new("Cycling", MuscleGroup.Legs, ExerciseType.Cardio),
                new("Swimming", MuscleGroup.FullBody, ExerciseType.Cardio),
            };


            await dbContext.Exercises.AddRangeAsync(defaultExercises);
            await dbContext.SaveChangesAsync();
        }

    }
}