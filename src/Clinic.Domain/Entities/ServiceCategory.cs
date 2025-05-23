using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public class ServiceCategory : BaseEntity, IAuditableEntity
{
    private ServiceCategory()
    {
    }

    private ServiceCategory(string name, TimeSpan duration)
    {
        Name = name;
        Duration = duration;
        IsActive = true;
    }

    public string Name { get; private set; }
    public TimeSpan Duration { get; private set; }
    public bool IsActive { get; private set; }
    public ICollection<Service> Services { get; private set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }

    public void Update(string name, TimeSpan duration)
    {
        Name = name;
        Duration = duration;
    }

    public static ServiceCategory Create(string name, TimeSpan duration)
    {
        return new ServiceCategory(name, duration);
    }
}