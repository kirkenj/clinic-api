using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.ConfirmSignUp;

public sealed class ConfirmPatientSignUpCommandHandler(
    ConfirmPatientSignUpCommandValidator validator,
    IPatientService patientService)
    : IRequestHandler<ConfirmPatientSignUpCommand, Result>
{
    public async Task<Result> Handle(ConfirmPatientSignUpCommand request,
        CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage));
        }

        return await patientService.ConfirmSignUp(request.Email, request.Otp, cancellationToken);
    }
}