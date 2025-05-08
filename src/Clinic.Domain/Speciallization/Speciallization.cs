using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Speciallization;
public sealed class Speciallization : BaseEntity, IAuditableEntity
{
    private Speciallization() { }

    public Speciallization(string name)
    {
        Name = name;
        IsActive = true;
    }

    public bool IsActive { get; private set; }
    public string Name { get; private set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }

    public void Update(string name)
    {
        Name = name;
    }

    public static Speciallization Create(string name)
        => new Speciallization(name);
}