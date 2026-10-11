using Clinic.Application.Responses.AppointmentResults;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Queries.GetByAppointmentId;

public sealed class GetAppointmentResultByAppointmentIdQuery : IRequest<Result<AppointmentResultResponse>>
{
    public uint AppointmentId { get; set; }
}