using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class ReceptionistErrors
{
    public static Error NotFound(uint id) => new("Receptionist.NotFound", $"Receptionist with id {id} was not found");
    public static Error NotActive(uint id) => new("Receptionist.NotActive", $"Receptionist with id {id} is not active");
    public static Error PhoneAlreadyTaken(string phoneNumber) => new("Receptionist.PhoneAlreadyTaken", $"Phone number '{phoneNumber}' is already taken by other receptionist");
    public static Error EmailAlreadyTaken(string email) => new("Receptionist.EmailAlreadyTaken", $"Email '{email}' is already taken by other receptionist");
}