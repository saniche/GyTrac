
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        return exerciseSeeds.Select(seed => new Exercise(
            seed.Name,
            Enum.Parse<MuscleGroup>(seed.PrimaryMuscleGroup, ignoreCase: true),
            Enum.Parse<ExerciseType>(seed.Type, ignoreCase: true),
            seed.Description)).ToList();
    }

    private sealed class ExerciseSeed
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("primaryMuscleGroup")]
        public string PrimaryMuscleGroup { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }
}