using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class PatientErrors
{
    public static Error NotFound(uint id) => new("Patient.NotFound", $"Patient with id {id} was not found");
    public static Error NotActive(uint id) => new("Patient.NotActive", $"Patient with id {id} is not active");
    public static Error PhoneAlreadyTaken(string phoneNumber) => new("Patient.PhoneAlreadyTaken", $"Phone number '{phoneNumber}' is already taken by other patient");
    public static Error EmailAlreadyTaken(string email) => new("Patient.EmailAlreadyTaken", $"Email '{email}' is already taken by other patient");
}