using GymTracker.Common.Dispatcher;
using GymTracker.Common.Pagination;
using GymTracker.Domain.Entities;
using GymTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Application.Exercises;

public interface IGyTracDbContext
{
    DbSet<Exercise> Exercises { get; }
}

public enum ExerciseGroupBy
{
    None = 0,
    Type = 1,
    MuscleGroup = 2
}

public sealed record ExerciseListItem(
    Guid Id,
    string Name,
    string? Description,
    string Type,
    string PrimaryMuscleGroup);

public sealed record ExerciseGroup(string Key, IReadOnlyList<ExerciseListItem> Items);

public sealed record AllExercisesResult(PaginatedResult<ExerciseListItem> PaginatedResult);

public sealed record GroupedExercisesResult(
    PaginatedResult<ExerciseGroup> PaginatedResult,
    ExerciseGroupBy GroupBy);

/// <summary>
/// Retrieves exercises with optional filtering, paging, and grouping.
/// </summary>
public sealed record GetAllExercisesQuery(
    PaginationRequest Pagination,
    ExerciseType? Type = null,
    MuscleGroup? MuscleGroup = null);

public sealed record GetGroupedExercisesQuery(
    PaginationRequest Pagination,
    ExerciseGroupBy GroupBy,
    ExerciseType? Type = null,
    MuscleGroup? MuscleGroup = null);

public sealed class GetAllExercisesHandler : IQueryHandler<GetAllExercisesQuery, AllExercisesResult>
{
    private readonly IGyTracDbContext _dbContext;

    public GetAllExercisesHandler(IGyTracDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AllExercisesResult> HandleAsync(GetAllExercisesQuery query, CancellationToken cancellationToken = default)
    {
        var exercises = ExerciseQueryHelpers.BuildExercisesQuery(_dbContext, query.Pagination.SearchValue.Value, query.Type, query.MuscleGroup);

        var totalCount = await exercises.CountAsync(cancellationToken);
        var skip = (query.Pagination.Page - 1) * query.Pagination.PageSize;
        var pageItems = await exercises
            .OrderBy(exercise => exercise.Name)
            .Skip(skip)
            .Take(query.Pagination.PageSize)
            .Select(exercise => ExerciseQueryHelpers.MapExercise(exercise))
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedResult<ExerciseListItem>(
            new QueryPage(query.Pagination.Page),
            new QueryPageSize(query.Pagination.PageSize),
            totalCount,
            pageItems);
        return new AllExercisesResult(paginatedResult);
    }
}

public sealed class GetGroupedExercisesHandler : IQueryHandler<GetGroupedExercisesQuery, GroupedExercisesResult>
{
    private readonly IGyTracDbContext _dbContext;

    public GetGroupedExercisesHandler(IGyTracDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GroupedExercisesResult> HandleAsync(GetGroupedExercisesQuery query, CancellationToken cancellationToken = default)
    {
        var exercises = ExerciseQueryHelpers.BuildExercisesQuery(_dbContext, query.Pagination.SearchValue.Value, query.Type, query.MuscleGroup);

        var totalCount = await exercises.CountAsync(cancellationToken);
        var skip = (query.Pagination.Page - 1) * query.Pagination.PageSize;
        var pageItems = await exercises
            .OrderBy(exercise => exercise.Name)
            .Skip(skip)
            .Take(query.Pagination.PageSize)
            .Select(exercise => ExerciseQueryHelpers.MapExercise(exercise))
            .ToListAsync(cancellationToken);

        var groupedItems = query.GroupBy switch
        {
            ExerciseGroupBy.Type => pageItems
                .GroupBy(item => item.Type)
                .Select(group => new ExerciseGroup(group.Key, group.ToList()))
                .ToList(),
            ExerciseGroupBy.MuscleGroup => pageItems
                .GroupBy(item => item.PrimaryMuscleGroup)
                .Select(group => new ExerciseGroup(group.Key, group.ToList()))
                .ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(query.GroupBy), query.GroupBy, null)
        };

        var paginatedResult = new PaginatedResult<ExerciseGroup>(
            new QueryPage(query.Pagination.Page),
            new QueryPageSize(query.Pagination.PageSize),
            totalCount,
            groupedItems);
        return new GroupedExercisesResult(paginatedResult, query.GroupBy);
    }

}

internal static class ExerciseQueryHelpers
{
    public static IQueryable<Exercise> BuildExercisesQuery(
        IGyTracDbContext dbContext,
        string? searchValue,
        ExerciseType? type,
        MuscleGroup? muscleGroup)
    {
        var normalizedName = searchValue?.Trim();
        var exercises = dbContext.Exercises.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(normalizedName))
        {
            var loweredName = normalizedName.ToLower();
            exercises = exercises.Where(exercise => EF.Functions.Like(exercise.Name, $"%{loweredName}%"));
        }

        if (type is not null)
        {
            exercises = exercises.Where(exercise => exercise.Type == type);
        }

        if (muscleGroup is not null)
        {
            exercises = exercises.Where(exercise => exercise.PrimaryMuscleGroup == muscleGroup);
        }

        return exercises;
    }

    public static ExerciseListItem MapExercise(Exercise exercise)
    {
        return new ExerciseListItem(
            exercise.Id,
            exercise.Name,
            exercise.Description,
            exercise.Type.ToFriendlyString(),
            exercise.PrimaryMuscleGroup.ToFriendlyString());
    }
}
