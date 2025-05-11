using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Doctor;

public static class DoctorErrors
{
    public static Error NotFound(uint id) => new("Doctor.NotFound", $"Doctor with id {id} was not found");
    public static Error NotActive(uint id) => new("Doctor.NotActive", $"Doctor with id {id} is not active");
    public static Error PhoneAlreadyTaken(string phoneNumber) => new("Doctor.PhoneAlreadyTaken", $"Phone number '{phoneNumber}' is already taken by other doctor");
    public static Error EmailAlreadyTaken(string email) => new("Doctor.EmailAlreadyTaken", $"Email '{email}' is already taken by other doctor");
}