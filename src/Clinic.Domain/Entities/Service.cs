using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public sealed class Service : BaseEntity, IAuditableEntity
{
    private Service()
    {
    }

    private Service(string name, decimal price,
        uint specializationId)
    {
        IsActive = true;
        Name = name;
        Price = price;
        SpecializationId = specializationId;
    }

    public bool IsActive { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; }
    public uint SpecializationId { get; private set; }
    public Specialization Specialization { get; private set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }


    public void Update(string name, decimal price,
        uint specializationId, uint categoryId)
    {
        Name = name;
        Price = price;
        SpecializationId = specializationId;
    }

    public static Service Create(string name, decimal price,
        uint specializationId)
    {
        return new Service(name, price, specializationId);
    }
}