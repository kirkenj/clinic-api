namespace Clinic.Application.Dtos.Office;
public sealed record OfficeResponse(
    uint Id,
    string Address,
    string PhoneNumber,
    bool IsActive
    )
{
    public static OfficeResponse FromOffice(Domain.Office.Office office) =>
        new(office.Id, office.Address, office.PhoneNumber, office.IsActive);

    public static IEnumerable<OfficeResponse> FromOfficeCollection(IEnumerable<Domain.Office.Office> offices) =>
            offices.Select(FromOffice);
}