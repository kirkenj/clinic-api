using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class BasicErrors
{
    public static Error NullOrEmpty(string parameterName)
    {
        return new Error($"Parameter '{parameterName}' can not be null, empty or whitespace");
    }
}