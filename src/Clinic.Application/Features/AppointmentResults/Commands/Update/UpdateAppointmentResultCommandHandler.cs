using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Commands.Update;

public sealed class UpdateAppointmentResultCommandHandler(
    IAppointmentResultRepository appointmentResultRepository,
    IAppointmentRepository appointmentRepository,
    IDateTimeProvider dateTimeProvider,
    UpdateAppointmentResultCommandValidator validator)
    : IRequestHandler<UpdateAppointmentResultCommand, Result>
{
    public async Task<Result> Handle(UpdateAppointmentResultCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var appointmentResult = await appointmentResultRepository.GetByIdAsync(request.Id, cancellationToken);
        if (appointmentResult is null)
        {
            return AppointmentResultErrors.NotFound(request.Id);
        }

        if (appointmentResult.AppointmentId != request.AppointmentId)
        {
            var newAppointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
            if (newAppointment is null)
            {
                return AppointmentErrors.NotFound(request.AppointmentId);
            }

            if (newAppointment.Status != AppointmentStatus.Confirmed)
            {
                return AppointmentErrors.InvalidStatus(newAppointment.Status, AppointmentStatus.Confirmed);
            }

            if (newAppointment.DateTimeStart.ToUniversalTime() > dateTimeProvider.UtcNow())
            {
                return AppointmentErrors.HasNotTakenPlace(newAppointment.Id);
            }

            if (await appointmentResultRepository.ExistsByAppointmentIdAsync(request.AppointmentId, cancellationToken))
            {
                return AppointmentResultErrors.AppointmentHasResult(request.AppointmentId);
            }
        }

        appointmentResult.Update(request.Complaints, request.Conclusion,
            request.Recommendations, request.AppointmentId);
        await appointmentResultRepository.UpdateAsync(appointmentResult, cancellationToken);
        return Result.Success();
    }
}