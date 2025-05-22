using Clinic.Application.Responses.Services;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Services.Queries.Get;

public sealed class GetServiceQueryHandler(IServiceRepository serviceRepository)
    : IRequestHandler<GetServiceQuery, Result<ServiceResponse>>
{
    public async Task<Result<ServiceResponse>> Handle(GetServiceQuery request, CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (service is null)
        {
            return Result.Failure<ServiceResponse>(
                ServiceErrors.NotFound(request.Id));
        }

        return ServiceResponse.FromService(service);
    }
}