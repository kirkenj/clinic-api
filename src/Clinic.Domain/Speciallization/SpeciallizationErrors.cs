using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Speciallization;
public static class SpeciallizationErrors
{
    public static Error NameAlreadyTaken(string name) => new("Office.NameAlreadyTaken", $"Name '{name}' is already taken by other speciallization");
    public static Error NotFound(uint id) => new("Speciallization.NotFound", $"Speciallization with id {id} was not found");
}