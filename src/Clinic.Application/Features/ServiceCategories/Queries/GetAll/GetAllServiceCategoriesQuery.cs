using Clinic.Application.Responses.ServiceCategories;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.ServiceCategories.Queries.GetAll;

public sealed class GetAllServiceCategoriesQuery : IRequest<Result<IEnumerable<ServiceCategoryResponse>>>
{
}