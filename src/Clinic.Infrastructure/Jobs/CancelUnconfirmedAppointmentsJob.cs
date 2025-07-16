using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Filters;
using Clinic.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Clinic.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class CancelUnconfirmedAppointmentsJob
    (ILogger<CancelUnconfirmedAppointmentsJob> logger,
    IDateTimeProvider dateTimeProvider,
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository,
    IEmailService emailService,
    IServiceRepository serviceService,
    IAppointmentRepository appointmentRepository)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var cutoffTime = dateTimeProvider.UtcNow();
        logger.LogInformation("Starting job to cancel unconfirmed appointments at {time}", cutoffTime);

        try
        {
            var appointments = await appointmentRepository.GetAppointmentsToOutdate(cutoffTime,
                context.CancellationToken);
            if (!appointments.Any())
            {
                logger.LogInformation("No unconfirmed appointments to cancel.");
                return;
            }

            var totalCount = appointments.Count();
            logger.LogInformation("Found {count} appointments to cancel.", totalCount);
            var appointmentsToNotify = appointments.Where(a => a.IsApprovedByUser).Distinct();

            var serviceIds = appointmentsToNotify.Select(a => a.ServiceId).Distinct().ToArray();
            var servicesForNotification = await serviceService.GetPaginatedAsync(
                new ServiceFilter
                {
                    Ids = serviceIds
                },
                1,
                serviceIds.Length,
                context.CancellationToken);

            var patientIds = appointmentsToNotify.Select(a => a.PatientId).Distinct().ToArray();
            var patientsToNotify = await patientRepository.GetPaginatedAsync(
                new PatientFilter
                {
                    Ids = patientIds
                },
                1,
                patientIds.Length,
                context.CancellationToken);

            var doctorIds = appointmentsToNotify.Select(a => a.DoctorId).Distinct().ToArray();
            var doctorsForNotification = await doctorRepository.GetPaginatedAsync(
                new DoctorFilter
                {
                    Ids = doctorIds
                },
                1,
                doctorIds.Length,
                context.CancellationToken);

            foreach (var appointment in appointments)
            {
                appointment.Update(
                    appointment.DoctorId,
                    appointment.PatientId,
                    appointment.ServiceId,
                    appointment.DateTimeStart,
                    appointment.DateTimeEnd,
                    appointment.IsApprovedByUser,
                    false,
                    AppointmentStatus.CancelledByClinic
                );
            }

            await appointmentRepository.UpdateRangeAsync(appointments, context.CancellationToken);

            await Task.WhenAll(appointmentsToNotify.Select(a =>
            {
                var patient = patientsToNotify.Items.FirstOrDefault(p => p.Id == a.PatientId);
                if (patient is null)
                {
                    return Task.CompletedTask;
                }

                var service = servicesForNotification.Items.FirstOrDefault(s => s.Id == a.ServiceId);
                if (service is null)
                {
                    return Task.CompletedTask;
                }

                var doctor = doctorsForNotification.Items.FirstOrDefault(d => d.Id == a.DoctorId);
                if (doctor is null)
                {
                    return Task.CompletedTask;
                }

                return emailService.SendAsync(patient.Email, "Appointment outdated",
                   $"Appointment: '{service.Name}' {service.Price} ({doctor.FirstName} {doctor.LastName}) " +
                   $"at {a.DateTimeStart} was outdated and cancelled.", context.CancellationToken);
            }));


            logger.LogInformation("Successfully cancelled {count} appointments.", totalCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while executing CancelUnconfirmedAppointmentsJob.");
        }
    }
}