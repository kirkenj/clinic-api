using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginatedForPatient;

public class GetPaginatedAppointmentsForPatientRequest : IRequest<Result<PaginatedCollection<AppointmentResponse>>>
{
    public uint PatientId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}