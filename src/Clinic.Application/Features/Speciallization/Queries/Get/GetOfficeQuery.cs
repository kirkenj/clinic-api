using Clinic.Application.Dtos.Speciallization;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Speciallization.Queries.Get;

public sealed record class GetSpeciallizationQuery(uint Id) : IRequest<Result<SpeciallizationDto>>
{ }