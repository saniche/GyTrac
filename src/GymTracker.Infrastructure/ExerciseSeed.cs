using System.Text.Json.Serialization;

namespace GymTracker.Infrastructure;


public static partial class DependencyInjection
{
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