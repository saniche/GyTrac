using GymTracker.Common.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GymTracker.Common.Dispatcher;

public sealed class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Dispatcher> _logger;

    public Dispatcher(IServiceProvider serviceProvider, ILogger<Dispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : notnull
    {
        await ValidateAsync(command, cancellationToken);

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        var operationName = typeof(TCommand).Name;
        var start = DateTime.UtcNow;

        _logger.LogInformation("Executing command {OperationName}", operationName);
        await handler.HandleAsync(command, cancellationToken);
        _logger.LogInformation("Command {OperationName} completed in {ElapsedMs}ms",
            operationName, (DateTime.UtcNow - start).TotalMilliseconds);
    }

    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : notnull
    {
        await ValidateAsync(command, cancellationToken);

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        var operationName = typeof(TCommand).Name;
        var start = DateTime.UtcNow;

        _logger.LogInformation("Executing command {OperationName}", operationName);
        var result = await handler.HandleAsync(command, cancellationToken);
        _logger.LogInformation("Command {OperationName} completed in {ElapsedMs}ms",
            operationName, (DateTime.UtcNow - start).TotalMilliseconds);

        return result;
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : notnull
    {
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        var operationName = typeof(TQuery).Name;
        var start = DateTime.UtcNow;

        _logger.LogInformation("Executing query {OperationName}", operationName);
        var result = await handler.HandleAsync(query, cancellationToken);
        _logger.LogInformation("Query {OperationName} completed in {ElapsedMs}ms",
            operationName, (DateTime.UtcNow - start).TotalMilliseconds);

        return result;
    }

    private async Task ValidateAsync<T>(T value, CancellationToken cancellationToken)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();
        if (validator is null) return;

        var result = await validator.ValidateAsync(value, cancellationToken);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
    }
}
