namespace GymTracker.Common.Validation;

public sealed class ValidationException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public ValidationException(IReadOnlyList<string> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
