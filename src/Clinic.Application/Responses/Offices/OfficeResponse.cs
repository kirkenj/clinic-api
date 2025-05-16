using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.Offices;

public sealed record OfficeResponse(
    uint Id,
    string Address,
    string PhoneNumber,
    bool IsActive
)
{
    public static OfficeResponse FromOffice(Office office)
    {
        return new OfficeResponse(
            office.Id,
            office.Address,
            office.PhoneNumber,
            office.IsActive
        );
    }

    public static IEnumerable<OfficeResponse> FromOfficeCollection(IEnumerable<Office> offices)
    {
        return offices.Select(FromOffice);
    }
}