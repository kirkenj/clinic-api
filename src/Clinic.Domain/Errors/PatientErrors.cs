using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class PatientErrors
{
    public static Error NotFound(uint id)
    {
        return new Error("Patient.NotFound", $"Patient with id {id} was not found");
    }

    public static Error NotActive(uint id)
    {
        return new Error("Patient.NotActive", $"Patient with id {id} is not active");
    }

    public static Error PhoneAlreadyTaken(string phoneNumber)
    {
        return new Error("Patient.PhoneAlreadyTaken",
            $"Phone number '{phoneNumber}' is already taken by other patient");
    }

    public static Error EmailAlreadyTaken(string email)
    {
        return new Error("Patient.EmailAlreadyTaken", $"Email '{email}' is already taken by other patient");
    }
}