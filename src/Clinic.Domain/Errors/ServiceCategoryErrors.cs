using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class ServiceCategoryErrors
{
    public static Error NotFound(uint id)
    {
        return new Error("ServiceCategory.NotFound", $"ServiceCategory with id {id} was not found");
    }

    public static Error NotActive(uint id)
    {
        return new Error("ServiceCategory.NotActive", $"ServiceCategory with id {id} is not active");
    }

    public static Error NameAlreadyTaken(string name)
    {
        return new Error("ServiceCategory.NameAlreadyTaken",
            $"Name '{name}' is already taken by other ServiceCategory");
    }
}