using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public sealed class Specialization : BaseEntity, IAuditableEntity
{
    private Specialization()
    {
    }

    public Specialization(string name)
    {
        Name = name;
        IsActive = true;
    }

    public bool IsActive { get; private set; }
    public string Name { get; private set; } = default!;
    public ICollection<Doctor> Doctors { get; private set; } = default!;
    public ICollection<Service> Services { get; private set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }

    public void Update(string name)
    {
        Name = name;
    }

    public static Specialization Create(string name)
    {
        return new Specialization(name);
    }
}