using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public sealed class Office : BaseEntity, IAuditableEntity
{
    private Office()
    {
    }

    private Office(string address, string registryPhoneNumber)
    {
        Address = address;
        PhoneNumber = registryPhoneNumber;
        IsActive = true;
    }

    public string Address { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public ICollection<Doctor> Doctors { get; private set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }

    public void Update(string address, string registryPhoneNumber)
    {
        Address = address;
        PhoneNumber = registryPhoneNumber;
    }

    public static Office Create(string address, string registryPhoneNumber)
    {
        return new Office(address, registryPhoneNumber);
    }
}