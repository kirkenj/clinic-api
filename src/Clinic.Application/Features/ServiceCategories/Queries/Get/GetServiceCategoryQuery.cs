using Clinic.Application.Responses.ServiceCategories;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.ServiceCategories.Queries.Get;

public sealed class GetServiceCategoryQuery : IRequest<Result<ServiceCategoryResponse>>
{
    public uint Id { get; set; }
}