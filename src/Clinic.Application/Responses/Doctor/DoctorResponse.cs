namespace Clinic.Application.Responses.Doctor;
public sealed record DoctorResponse(
    uint Id,
    string FirstName,
    string LastdName,
    string MiddleName,
    string Email,
    string PhoneNumber,
    uint specializationId,
    uint officeId,
    bool IsActive
    )
{
    public static DoctorResponse FromDoctor(Domain.Doctor.Doctor doctor) =>
        new(doctor.Id, doctor.FirstName, doctor.MiddleName, doctor.LastName, doctor.Email, doctor.PhoneNumber, doctor.OfficeId, doctor.SpecializationId, doctor.IsActive);

    public static IEnumerable<DoctorResponse> FromDoctorCollection(IEnumerable<Domain.Doctor.Doctor> doctors) =>
            doctors.Select(FromDoctor);
}