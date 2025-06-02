using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class DoctorErrors
{
    public static Error NotFound(uint id)
    {
        return new Error("Doctor.NotFound", $"Doctor with id {id} was not found");
    }

    public static Error NotActive(uint id)
    {
        return new Error("Doctor.NotActive", $"Doctor with id {id} is not active");
    }

    public static Error PhoneAlreadyTaken(string phoneNumber)
    {
        return new Error("Doctor.PhoneAlreadyTaken", $"Phone number '{phoneNumber}' is already taken by other doctor");
    }

    public static Error EmailAlreadyTaken(string email)
    {
        return new Error("Doctor.EmailAlreadyTaken", $"Email '{email}' is already taken by other doctor");
    }

    public static Error NoDoctorsForGivenSpecialization(uint specializationId)
    {
        return new Error("Doctor.NoDoctorsForGivenSpecialization", $"Doctors with specialization id '{specializationId}' were not found");
    }
}