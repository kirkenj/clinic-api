using Clinic.Application.Dtos.Specialization;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specialization.Queries.Get;

public sealed record GetSpecializationQuery(uint Id) : IRequest<Result<SpecializationResponse>>
{ }