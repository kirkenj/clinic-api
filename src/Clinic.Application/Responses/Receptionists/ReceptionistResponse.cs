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
    public static ReceptionistResponse FromReceptionist(Receptionist receptionist)
    {
        return new ReceptionistResponse(
            receptionist.Id,
            receptionist.FirstName,
            receptionist.MiddleName,
            receptionist.LastName,
            receptionist.Email,
            receptionist.PhoneNumber,
            receptionist.IsActive
        );
    }
}