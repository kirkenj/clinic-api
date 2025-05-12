using Clinic.Application.Responses.Specializations;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specializations.Queries.Get;

public sealed record GetSpecializationQuery(uint Id) : IRequest<Result<SpecializationResponse>>
{ }