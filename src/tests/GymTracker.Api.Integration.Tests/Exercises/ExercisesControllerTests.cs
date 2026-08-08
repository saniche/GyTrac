
using System.Net.Http.Json;
using GymTracker.Application.Exercises;
using Microsoft.AspNetCore.Http;

namespace GymTracker.Api.Integration.Tests.Exercises;

public class ExercisesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ExercisesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/exercises");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
    }

    //tests results of GetAll endpoint with query parameters
    [Fact]
    public async Task GetAll_WithQueryParameters_ReturnsFilteredResults()
    {
        // Arrange
        var queryParams = new Dictionary<string, string>
        {
            { "name", "press" },
            { "type", "Strength" },
            { "muscleGroup", "Chest" },
            { "page", "1" },
            { "pageSize", "5" }
        };
        var queryString = QueryString.Create(queryParams).ToUriComponent();

        // Act
        var response = await _client.GetAsync($"/api/exercises{queryString}");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var result = await response.Content.ReadFromJsonAsync<AllExercisesResult>();
        Assert.NotNull(result);
        Assert.NotNull(result.PaginatedResult);
        Assert.True(result.PaginatedResult.Count > 0);

    }
}