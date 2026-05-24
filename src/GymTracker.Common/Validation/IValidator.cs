namespace GymTracker.Common.Validation;

public interface IValidator<in T>
{
    Task<ValidationResult> ValidateAsync(T value, CancellationToken cancellationToken = default);
}
