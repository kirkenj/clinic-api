using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class ReceptionistErrors
{
    public static Error NotFound(uint id)
    {
        return new Error("Receptionist.NotFound", $"Receptionist with id {id} was not found");
    }

    public static Error NotActive(uint id)
    {
        return new Error("Receptionist.NotActive", $"Receptionist with id {id} is not active");
    }

    public static Error PhoneAlreadyTaken(string phoneNumber)
    {
        return new Error("Receptionist.PhoneAlreadyTaken",
            $"Phone number '{phoneNumber}' is already taken by other receptionist");
    }

    public static Error EmailAlreadyTaken(string email)
    {
        return new Error("Receptionist.EmailAlreadyTaken", $"Email '{email}' is already taken by other receptionist");
    }
}