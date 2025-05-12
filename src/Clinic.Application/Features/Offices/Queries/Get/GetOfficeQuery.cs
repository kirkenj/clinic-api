using Clinic.Application.Responses.Offices;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Offices.Queries.Get;

public sealed class GetOfficeQuery : IRequest<Result<OfficeResponse>>
{
    public uint Id { get; set; }
}