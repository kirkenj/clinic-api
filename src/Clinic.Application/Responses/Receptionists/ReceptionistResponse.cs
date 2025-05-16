using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.Receptionists;

public sealed record ReceptionistResponse(
    uint Id,
    string FirstName,
    string LastName,
    string MiddleName,
    string Email,
    string PhoneNumber,
    bool IsActive
)
{
    public static ReceptionistResponse FromReceptionist(Receptionist Receptionist)
    {
        return new ReceptionistResponse(
            Receptionist.Id,
            Receptionist.FirstName,
            Receptionist.MiddleName,
            Receptionist.LastName,
            Receptionist.Email,
            Receptionist.PhoneNumber,
            Receptionist.IsActive
        );
    }

    public static IEnumerable<ReceptionistResponse> FromReceptionistCollection(IEnumerable<Receptionist> Receptionists)
    {
        return Receptionists.Select(FromReceptionist);
    }
}