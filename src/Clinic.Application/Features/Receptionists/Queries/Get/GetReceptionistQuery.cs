using Clinic.Application.Responses.Receptionists;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Queries.Get;

public sealed class GetReceptionistQuery : IRequest<Result<ReceptionistResponse>>
{
    public uint Id { get; set; }
}