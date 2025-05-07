namespace Clinic.Domain.Abstractions;

public record Error(string Code, string? Description = null)
{
    public static readonly Error None = new(string.Empty);
    public static readonly Error NullValue = new("Value is null");

    public static Error ValidationError(IEnumerable<string> errorMessages) => new(string.Join(";", errorMessages));

    public static implicit operator Result(Error error) => Result.Failure(error);
}