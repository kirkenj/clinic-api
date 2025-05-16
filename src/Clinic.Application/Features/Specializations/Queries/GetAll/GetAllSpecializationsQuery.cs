using Clinic.Application.Responses.Specializations;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specializations.Queries.GetAll;

public sealed class GetAllSpecializationsQuery : IRequest<Result<IEnumerable<SpecializationResponse>>>
{
}