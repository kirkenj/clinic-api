using Clinic.Application.Abstractions;
using Clinic.Application.Extensions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.Update.Receptionist;

public sealed class ReceptionistUpdatePatientCommandHandler(
    IPatientService patientService,
    IUserAccessService userAccessService,
    ReceptionistUpdatePatientCommandValidator validator)
    : IRequestHandler<ReceptionistUpdatePatientCommand, Result>
{
    public async Task<Result> Handle(ReceptionistUpdatePatientCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors
                .Select(e => e.ErrorMessage));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Receptionist], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return userAccessResult.Error;
        }

        return await patientService.UpdateAsync(request.Id, request.LastName, request.FirstName,
            request.MiddleName, request.PhoneNumber, request.Email, cancellationToken);
    }
}