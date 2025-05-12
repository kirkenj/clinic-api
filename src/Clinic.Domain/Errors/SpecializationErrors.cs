using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;
public static class SpecializationErrors
{
    public static Error NameAlreadyTaken(string name) => new("Office.NameAlreadyTaken", $"Name '{name}' is already taken by other specialization");
    public static Error NotActive(uint id) => new("Specialization.NotActive", $"Specialization with id {id} is not active");
    public static Error NotFound(uint id) => new("Specialization.NotFound", $"Specialization with id {id} was not found");
}