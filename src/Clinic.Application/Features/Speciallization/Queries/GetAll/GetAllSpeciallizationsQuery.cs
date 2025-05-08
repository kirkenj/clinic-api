using Clinic.Application.Dtos.Speciallization;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Speciallization.Queries.GetAll;
public sealed class GetAllSpeciallizationsQuery : IRequest<Result<IEnumerable<SpeciallizationResponse>>>
{ }