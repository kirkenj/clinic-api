using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.Services;

public sealed record ServiceResponse(
    uint Id,
    string Name, 
    decimal Price,
    uint SpecializationId, 
    bool IsActive
)
{
    public static ServiceResponse FromService(Service service)
    {
        return new ServiceResponse(
            service.Id,
            service.Name,
            service.Price,
            service.SpecializationId,
            service.IsActive
        );
    }

    public static IEnumerable<ServiceResponse> FromServiceCollection(IEnumerable<Service> services)
    {
        return services.Select(FromService);
    }
}