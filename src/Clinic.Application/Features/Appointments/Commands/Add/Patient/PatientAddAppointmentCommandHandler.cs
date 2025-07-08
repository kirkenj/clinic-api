using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.Add.Patient;

public sealed class PatientAddAppointmentCommandHandler(
    PatientAddAppointmentCommandValidator validator,
    IAppointmentService appointmentService,
    IUserAccessService userAccessService)
    : IRequestHandler<PatientAddAppointmentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(PatientAddAppointmentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Patient], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return Result.Failure<string>(userAccessResult.Error);
        }

        return await appointmentService.AddByPatient(userAccessResult.Value.Id, request.DoctorId, 
            request.ServiceId, request.DateTime, cancellationToken);
    }
}