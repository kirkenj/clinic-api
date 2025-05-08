using Clinic.Application.Dtos.Specialization;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specialization.Queries.GetAll;
public sealed record GetAllSpecializationsQuery : IRequest<Result<IEnumerable<SpecializationResponse>>>
{ }