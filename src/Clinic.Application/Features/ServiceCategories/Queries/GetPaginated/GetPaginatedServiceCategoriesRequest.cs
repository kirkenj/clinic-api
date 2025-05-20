using Clinic.Application.Responses.ServiceCategories;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.ServiceCategories.Queries.GetPaginated;

public class GetPaginatedServiceCategoriesRequest : IRequest<Result<PaginatedCollection<ServiceCategoryResponse>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}