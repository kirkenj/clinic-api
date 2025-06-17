using Clinic.Application.Abstractions;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.CancelByClinic;

public sealed class CancelAppointmentByClinicCommandHandler(
    IAppointmentRepository appointmentRepository,
    IDoctorRepository doctorRepository,
    IServiceRepository serviceRepository,
    IPatientRepository patientRepository,
    IEmailService emailService,
    CancelAppointmentByClinicCommandValidator validator)
    : IRequestHandler<CancelAppointmentByClinicCommand, Result>
{
    public async Task<Result> Handle(CancelAppointmentByClinicCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var appointment = await appointmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.Id);
        }

        appointment.Update(appointment.DoctorId, appointment.PatientId, appointment.ServiceId,
            appointment.DateTimeStart, appointment.DateTimeEnd, appointment.IsApprovedByUser,
            false, AppointmentStatus.CancelledByClinic);


        await appointmentRepository.UpdateAsync(appointment, cancellationToken);

        var patient = await patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(appointment.PatientId);
        }

        var doctor = await doctorRepository.GetByIdAsync(appointment.DoctorId, cancellationToken);
        if (doctor is null)
        {
            return DoctorErrors.NotFound(appointment.DoctorId);
        }

        var service = await serviceRepository.GetByIdAsync(appointment.ServiceId, cancellationToken);
        if (service is null)
        {
            return ServiceErrors.NotFound(appointment.ServiceId);
        }

        await Task.WhenAll(
            emailService.SendAsync(patient.Email, Constants.SubjectAppointmentCancelled,
                EmailBodyBuilder.BuildCancelledAppointmentContent(service.Name, doctor.FirstName, doctor.LastName,
                    appointment.DateTimeStart),
                cancellationToken),
            emailService.SendAsync(doctor.Email, Constants.SubjectAppointmentCancelled,
                EmailBodyBuilder.BuildCancelledAppointmentContent(service.Name, patient.FirstName, patient.LastName,
                    appointment.DateTimeStart),
                cancellationToken));
        return Result.Success();
    }
}