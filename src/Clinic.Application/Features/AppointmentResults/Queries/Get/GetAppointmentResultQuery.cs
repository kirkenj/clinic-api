using Clinic.Application.Responses.AppointmentResults;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Queries.Get;

public sealed class GetAppointmentResultQuery : IRequest<Result<AppointmentResultResponse>>
{
    public uint Id { get; set; }
}