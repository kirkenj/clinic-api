using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class ServiceErrors
{
    public static Error NotFound(uint id)
    {
        return new Error("Service.NotFound", $"Service with id {id} was not found");
    }

    public static Error NotActive(uint id)
    {
        return new Error("Service.NotActive", $"Service with id {id} is not active");
    }

    public static Error NameAlreadyTaken(string name)
    {
        return new Error("Service.NameAlreadyTaken", $"Name '{name}' is already taken by other service");
    }

    public static Error PriceOutOfRange(decimal minimalValue, decimal maximalValue)
    {
        return new Error("Service.PriceOutOfRange", $"Price has to be in range between '{minimalValue}' and '{maximalValue}'.");
    }
}