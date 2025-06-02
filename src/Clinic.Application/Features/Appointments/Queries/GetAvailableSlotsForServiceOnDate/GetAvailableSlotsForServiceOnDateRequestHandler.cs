using Clinic.Application.Extensions;
using Clinic.Application.Responses.Appointments;
using Clinic.Application.Services.SlotService;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.GetAvailableSlotsForServiceOnDate;

public sealed class GetAvailableSlotsForServiceOnDateRequestHandler(
    IServiceRepository serviceRepository,
    IDoctorRepository doctorRepository,
    IAppointmentRepository appointmentRepository,
    ISlotService slotService)
    : IRequestHandler<GetAvailableSlotsForServiceOnDateRequest, Result<IEnumerable<SlotAndAvailableDoctors>>>
{
    public async Task<Result<IEnumerable<SlotAndAvailableDoctors>>> Handle(
        GetAvailableSlotsForServiceOnDateRequest request,
        CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetByIdAsync(request.ServiceId, cancellationToken);
        if (service is null)
        {
            return Result.Failure<IEnumerable<SlotAndAvailableDoctors>>
                (ServiceErrors.NotFound(request.ServiceId));
        }

        if (!service.IsActive)
        {
            return Result.Failure<IEnumerable<SlotAndAvailableDoctors>>
                (ServiceErrors.NotActive(request.ServiceId));
        }

        var doctors = (await doctorRepository
                .GetBySpecializationAsync(service.SpecializationId, cancellationToken))
            .ToList();
        if (doctors.Count == 0)
        {
            return Result.Failure<IEnumerable<SlotAndAvailableDoctors>>
                (DoctorErrors.NoDoctorsForGivenSpecialization(service.SpecializationId));
        }

        var doctorIds = doctors.Select(d => d.Id);
        var slots = slotService.GetSlots(request.Date);
        var appointmentsForDay = await appointmentRepository
            .GetAppointmentsForSpecializationOnDate
                (request.Date, service.SpecializationId, cancellationToken);
        var slotDoctors = slots.Select(s => new { Slot = s, Doctors = doctorIds })
            .ToArray();
        var result = slotDoctors.Select(sd =>
        {
            var takenDoctors = appointmentsForDay.Where
                    (a => sd.Slot.IntersectsWith(a.DateTimeStart, a.DateTimeEnd))
                .Select(a => a.DoctorId);

            return new SlotAndAvailableDoctors(sd.Slot, sd.Doctors.Except(takenDoctors));
        });

        result = result.Where(r => r.AvailableDoctors.Any());

        return Result.Success(result);
    }
}