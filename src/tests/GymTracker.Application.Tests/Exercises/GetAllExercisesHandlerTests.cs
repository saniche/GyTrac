using GymTracker.Application.Exercises;
using GymTracker.Common.Pagination;
using GymTracker.Domain.Entities;
using GymTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Application.Tests.Exercises;

public sealed class GetAllExercisesHandlerTests
{
    [Fact]
    public async Task HandleAsync_filters_orders_and_pages_results()
    {
        await using var context = CreateContext();
        await SeedAsync(context);

        var handler = new GetAllExercisesHandler(context);

        var result = await handler.HandleAsync(new GetAllExercisesQuery(
            new PaginationRequest(new SearchValue("press"), new QueryPage(1), new QueryPageSize(1))));

        Assert.Equal(2, result.PaginatedResult.TotalCount);
        Assert.Single(result.PaginatedResult.Data);
        Assert.Equal("Bench Press", result.PaginatedResult.Data.First().Name);
        Assert.Equal(1, result.PaginatedResult.Page);
        Assert.Equal(1, result.PaginatedResult.PageSize);
    }

    [Fact]
    public async Task HandleAsync_groups_paged_results_by_muscle_group()
    {
        await using var context = CreateContext();
        await SeedAsync(context);

        var handler = new GetGroupedExercisesHandler(context);

        var result = await handler.HandleAsync(new GetGroupedExercisesQuery(
            new PaginationRequest(new SearchValue(string.Empty), new QueryPage(1), new QueryPageSize(3)),
            ExerciseGroupBy.MuscleGroup));

        Assert.Equal(4, result.PaginatedResult.TotalCount);
        Assert.Equal(ExerciseGroupBy.MuscleGroup, result.GroupBy);
        Assert.Equal(2, result.PaginatedResult.Data.Count());
        Assert.Collection(
            result.PaginatedResult.Data,
            group =>
            {
                Assert.Equal("Chest", group.Key);
                Assert.Single(group.Items);
                Assert.Equal("Bench Press", group.Items[0].Name);
            },
            group =>
            {
                Assert.Equal("Back", group.Key);
                Assert.Equal(2, group.Items.Count);
                Assert.Equal("Bent Over Row", group.Items[0].Name);
                Assert.Equal("Pull-Up", group.Items[1].Name);
            });
    }

    private static TestGyTracDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestGyTracDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestGyTracDbContext(options);
    }

    private static async Task SeedAsync(TestGyTracDbContext context)
    {
        context.Exercises.AddRange(
            new Exercise("Bench Press", MuscleGroup.Chest, ExerciseType.Strength),
            new Exercise("Shoulder Press", MuscleGroup.Shoulders, ExerciseType.Strength),
            new Exercise("Pull-Up", MuscleGroup.Back, ExerciseType.Bodyweight),
            new Exercise("Bent Over Row", MuscleGroup.Back, ExerciseType.Strength));

        await context.SaveChangesAsync();
    }

    private sealed class TestGyTracDbContext(DbContextOptions<TestGyTracDbContext> options)
        : DbContext(options), IGyTracDbContext
    {
        public DbSet<Exercise> Exercises => Set<Exercise>();
    }
}