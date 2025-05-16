using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class SpecializationErrors
{
    public static Error NameAlreadyTaken(string name)
    {
        return new Error("Office.NameAlreadyTaken", $"Name '{name}' is already taken by other specialization");
    }

    public static Error NotActive(uint id)
    {
        return new Error("Specialization.NotActive", $"Specialization with id {id} is not active");
    }

    public static Error NotFound(uint id)
    {
        return new Error("Specialization.NotFound", $"Specialization with id {id} was not found");
    }
}