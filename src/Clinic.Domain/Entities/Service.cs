using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public sealed class Service : BaseEntity, IAuditableEntity
{
    private Service()
    {
    }

    private Service(string name, decimal price,
        uint specializationId, uint serviceCategoryId)
    {
        IsActive = true;
        Name = name;
        Price = price;
        SpecializationId = specializationId;
        ServiceCategoryId = serviceCategoryId;
    }

    public bool IsActive { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; }
    public uint SpecializationId { get; private set; }
    public Specialization Specialization { get; private set; } = default!;
    public uint ServiceCategoryId { get; private set; }
    public ServiceCategory ServiceCategory { get; private set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }


    public void Update(string name, decimal price,
        uint specializationId, uint categoryId)
    {
        Name = name;
        Price = price;
        SpecializationId = specializationId;
        ServiceCategoryId = categoryId;
    }

    public static Service Create(string name, decimal price,
        uint specializationId, uint categoryId)
    {
        return new Service(name, price, specializationId, categoryId);
    }
}