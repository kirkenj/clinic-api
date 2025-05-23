using Clinic.Application.Responses.Services;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Services.Queries.Get;

public sealed class GetServiceQuery : IRequest<Result<ServiceResponse>>
{
    public uint Id { get; set; }
}