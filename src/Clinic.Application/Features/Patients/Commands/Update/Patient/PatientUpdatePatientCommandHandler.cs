using Clinic.Application.Abstractions;
using Clinic.Application.Extensions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.Update.Patient;

public sealed class PatientUpdatePatientCommandHandler(
    IPatientService patientService,
    IUserAccessService userAccessService,
    PatientUpdatePatientCommandValidator validator)
    : IRequestHandler<PatientUpdatePatientCommand, Result>
{
    public async Task<Result> Handle(PatientUpdatePatientCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors
                .Select(e => e.ErrorMessage));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Patient], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return userAccessResult.Error;
        }

        return await patientService.UpdateAsync(userAccessResult.Value.Id, request.LastName, request.FirstName,
            request.MiddleName, request.PhoneNumber, request.Email, cancellationToken);
    }
}