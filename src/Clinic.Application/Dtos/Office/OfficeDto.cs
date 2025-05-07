namespace Clinic.Application.Dtos.Office;
public sealed class OfficeDto
{
    public uint Id { get; private set; }
    public string Address { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;
    public bool IsActive { get; private set; }

    public static OfficeDto FromOffice(Domain.Office.Office office) => new()
    {
        Id = office.Id,
        Address = office.Address,
        PhoneNumber = office.PhoneNumber,
        IsActive = office.IsActive
    };
}