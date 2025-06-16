using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Commands.Add;

public sealed class AddAppointmentResultCommandHandler(
    IAppointmentResultRepository appointmentResultRepository,
    IAppointmentRepository appointmentRepository,
    IDateTimeService dateTimeService,
    AddAppointmentResultCommandValidator validator)
    : IRequestHandler<AddAppointmentResultCommand, Result>
{
    public async Task<Result> Handle(AddAppointmentResultCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.AppointmentId);
        }

        if (appointment.Status != AppointmentStatus.Confirmed)
        {
            return AppointmentErrors.InvalidStatus(appointment.Status, AppointmentStatus.Confirmed);
        }

        if (appointment.DateTimeStart.ToUniversalTime() > dateTimeService.UtcNow())
        {
            return AppointmentErrors.HasNotTakenPlace(appointment.Id);
        }

        if (await appointmentResultRepository.ExistsByAppointmentIdAsync(request.AppointmentId, cancellationToken))
        {
            return AppointmentResultErrors.AppointmentHasResult(request.AppointmentId);
        }
        
        var appointmentResult = AppointmentResult.Create(request.Complaints, request.Conclusion, 
            request.Recommendations, request.AppointmentId);
        await appointmentResultRepository.AddAsync(appointmentResult, cancellationToken);
        return Result.Success();
    }
}