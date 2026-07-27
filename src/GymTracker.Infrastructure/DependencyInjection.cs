
using System.Reflection;
using System.Text.Json;
using GymTracker.Domain.Entities;
using GymTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.Infrastructure;


/// <summary>
/// Provides extension methods for configuring and initializing the infrastructure layer of the application.
/// </summary>
public static partial class DependencyInjection
{
    private const string ExercisesResourceName = "GymTracker.Infrastructure.Resources.exercises.json";

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
            var defaultExercises = await LoadDefaultExercisesAsync();

            await dbContext.Exercises.AddRangeAsync(defaultExercises);
            await dbContext.SaveChangesAsync();
        }

    }

    private static async Task<List<Exercise>> LoadDefaultExercisesAsync()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ExercisesResourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{ExercisesResourceName}' was not found.");

        var exerciseSeeds = await JsonSerializer.DeserializeAsync<List<ExerciseSeed>>(stream)
            ?? throw new InvalidOperationException("Exercise seed data could not be read.");

        return exerciseSeeds.Select(ExerciseSeedToExercise).ToList();
    }

    private static Exercise ExerciseSeedToExercise(ExerciseSeed seed)
    {
        return new Exercise(
            seed.Name,
            Enum.TryParse<MuscleGroup>(seed.PrimaryMuscleGroup, ignoreCase: true, out var primaryMuscleGroup) ? primaryMuscleGroup
                : throw new InvalidOperationException($"Invalid muscle group: {seed.PrimaryMuscleGroup} for exercise: {seed.Name}"),
            Enum.TryParse<ExerciseType>(seed.Type, ignoreCase: true, out var exerciseType) ? exerciseType
                : throw new InvalidOperationException($"Invalid exercise type: {seed.Type} for exercise: {seed.Name}"),
            seed.Description);
    }
}