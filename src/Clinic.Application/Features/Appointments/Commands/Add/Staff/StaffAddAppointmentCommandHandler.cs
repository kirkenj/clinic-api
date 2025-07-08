using Clinic.Application.Abstractions;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.Add.Staff;

public sealed class StaffAddAppointmentCommandHandler(
    StaffAddAppointmentCommandValidator validator,
    IAppointmentService appointmentService,
    IUserAccessService userAccessService)
    : IRequestHandler<StaffAddAppointmentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(StaffAddAppointmentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            Constants.StaffRoles, cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return Result.Failure<string>(userAccessResult.Error);
        }

        return await appointmentService.AddByPatient(request.PatientId, request.DoctorId, request.ServiceId, 
            request.DateTime, cancellationToken);
    }
}