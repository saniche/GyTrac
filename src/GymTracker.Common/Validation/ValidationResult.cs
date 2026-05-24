namespace GymTracker.Common.Validation;

public sealed class ValidationResult
{
    public static readonly ValidationResult Success = new(true, []);

    public bool IsValid { get; }
    public IReadOnlyList<string> Errors { get; }

    public ValidationResult(bool isValid, IReadOnlyList<string> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }

    public static ValidationResult Failure(params string[] errors) =>
        new(false, errors);
}
