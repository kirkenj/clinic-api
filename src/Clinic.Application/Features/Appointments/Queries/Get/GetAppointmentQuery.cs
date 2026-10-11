using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.Get.Staff;

public sealed class GetAppointmentQuery : IRequest<Result<AppointmentResponse>>
{
    public uint Id { get; set; }
}