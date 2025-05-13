using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.Patients;
public sealed record PatientResponse(
    uint Id,
    string FirstName,
    string LastName,
    string MiddleName,
    string Email,
    string PhoneNumber,
    bool IsActive
    )
{
    public static PatientResponse FromPatient(Patient Patient) =>
        new(
            Patient.Id,
            Patient.FirstName,
            Patient.MiddleName,
            Patient.LastName,
            Patient.Email,
            Patient.PhoneNumber,
            Patient.IsActive
            );

    public static IEnumerable<PatientResponse> FromPatientCollection(IEnumerable<Patient> Patients) =>
            Patients.Select(FromPatient);
}