using GymTracker.Application.Exercises;
using GymTracker.Common.Dispatcher;
using GymTracker.Common.Pagination;
using GymTracker.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.Api.Controllers;

[ApiController]
[Route("api/exercises")]
public sealed class ExercisesController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public ExercisesController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    public async Task<ActionResult<AllExercisesResult>> GetAll(
        [FromQuery] string? name = null,
        [FromQuery] ExerciseType? type = null,
        [FromQuery] MuscleGroup? muscleGroup = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var paginationRequest = new PaginationRequest(
            new SearchValue(name),
            new QueryPage(page),
            new QueryPageSize(pageSize));

        var result = await _dispatcher.QueryAsync<GetAllExercisesQuery, AllExercisesResult>(
            new GetAllExercisesQuery(paginationRequest, type, muscleGroup),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("grouped")]
    public async Task<ActionResult<GroupedExercisesResult>> GetGrouped(
        [FromQuery] ExerciseGroupBy groupBy,
        [FromQuery] string? name = null,
        [FromQuery] ExerciseType? type = null,
        [FromQuery] MuscleGroup? muscleGroup = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var paginationRequest = new PaginationRequest(
            new SearchValue(name),
            new QueryPage(page),
            new QueryPageSize(pageSize));

        var result = await _dispatcher.QueryAsync<GetGroupedExercisesQuery, GroupedExercisesResult>(
            new GetGroupedExercisesQuery(paginationRequest, groupBy, type, muscleGroup),
            cancellationToken);

        return Ok(result);
    }
}