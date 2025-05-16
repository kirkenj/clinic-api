using Clinic.Application.Responses.Specializations;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specializations.Queries.Get;

public sealed class GetSpecializationQuery : IRequest<Result<SpecializationResponse>>
{
    public uint Id { get; set; }
}