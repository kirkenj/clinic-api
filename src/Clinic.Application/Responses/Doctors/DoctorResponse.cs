using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.Doctors;

public sealed record DoctorResponse(
    uint Id,
    string FirstName,
    string LastName,
    string MiddleName,
    string Email,
    string PhoneNumber,
    uint SpecializationId,
    uint OfficeId,
    bool IsActive
)
{
    public static DoctorResponse FromDoctor(Doctor doctor)
    {
        return new DoctorResponse(
            doctor.Id,
            doctor.FirstName,
            doctor.MiddleName,
            doctor.LastName,
            doctor.Email,
            doctor.PhoneNumber,
            doctor.OfficeId,
            doctor.SpecializationId,
            doctor.IsActive
        );
    }
}