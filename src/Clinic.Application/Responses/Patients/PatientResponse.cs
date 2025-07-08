using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.Patients;

public sealed record PatientResponse(
    uint Id,
    string FirstName,
    string LastName,
    string MiddleName,
    string Email,
    string PhoneNumber
)
{
    public static PatientResponse FromPatient(Patient patient)
    {
        return new PatientResponse(
            patient.Id,
            patient.FirstName,
            patient.MiddleName,
            patient.LastName,
            patient.Email,
            patient.PhoneNumber
        );
    }
}