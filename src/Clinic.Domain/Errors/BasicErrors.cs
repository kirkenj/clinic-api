using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class BasicErrors
{
    public static Error NullOrEmpty(string parameterName)
    {
        return new Error("BasicErrors.NullOrEmpty", $"Parameter '{parameterName}' can not be null, empty or whitespace");
    }

    public static Error NoAccess()
    {
        return new Error("BasicErrors.NoAccess");
    }

    public static Error InvalidToken()
    {
        return new Error("BasicErrors.InvalidToken");
    }
}