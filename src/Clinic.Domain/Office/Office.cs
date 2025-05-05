using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Office;

public sealed class Office : BaseEntity, IAuditableEntity
{
    private Office() { }

    private Office(string address, string registryPhoneNumber, string photo, string photoId)
    {
        Address = address;
        RegistryPhoneNumber = registryPhoneNumber;
        Photo = photo;
        PhotoId = photoId;
        IsActive = true;
    }

    public string Address { get; private set; } = string.Empty;
    public string RegistryPhoneNumber { get; private set; } = string.Empty;

    public string Photo { get; private set; } = default!;
    public string PhotoId { get; private set; } = default!;

    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }

    public void Update(string address, string registryPhoneNumber, string photo, string photoId)
    {
        Address = address;
        RegistryPhoneNumber = registryPhoneNumber;
        Photo = photo;
        PhotoId = photoId;
    }

    public static Office Create(string address, string registryPhoneNumber, string photo, string photoId)
        => new(address, registryPhoneNumber, photo, photoId);
}