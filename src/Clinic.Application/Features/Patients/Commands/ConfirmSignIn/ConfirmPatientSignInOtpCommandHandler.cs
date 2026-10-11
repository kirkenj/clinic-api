using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.ConfirmSignIn;

public sealed class ConfirmPatientSignInOtpCommandHandler(
    ConfirmPatientSignInOtpCommandValidator validator,
    IPatientService patientService)
    : IRequestHandler<ConfirmPatientSignInOtpCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConfirmPatientSignInOtpCommand request,
        CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        return await patientService.ConfirmSignIn(request.Email, request.Otp, cancellationToken);
    }
}