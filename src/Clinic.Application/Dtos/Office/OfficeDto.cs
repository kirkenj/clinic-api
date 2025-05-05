namespace Clinic.Application.Dtos.Office;
public sealed class OfficeDto
{
    public uint Id { get; private set; }
    public string Address { get; private set; } = default!;
    public string RegistryPhoneNumber { get; private set; } = default!;
    public string Photo { get; private set; } = default!;
    public string PhotoId { get; private set; } = default!;
    public bool IsActive { get; private set; }

    public static OfficeDto FromOffice(Domain.Office.Office office) => new OfficeDto
    {
        Id = office.Id,
        Address = office.Address,
        RegistryPhoneNumber = office.RegistryPhoneNumber,
        Photo = office.Photo,
        PhotoId = office.PhotoId,
        IsActive = office.IsActive
    };
}