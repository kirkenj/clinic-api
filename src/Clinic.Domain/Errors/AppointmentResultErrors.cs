using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class AppointmentResultErrors
{
    public static Error NotFound(uint id)
    {
        return new Error("AppointmentResult.NotFound", $"AppointmentResult with id {id} was not found");
    }
}