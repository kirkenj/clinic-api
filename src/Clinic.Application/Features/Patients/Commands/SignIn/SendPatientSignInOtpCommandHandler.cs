using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.SignIn;

public class SendPatientSignInOtpCommandHandler(
    SendPatientSignInOtpCommandValidator validator,
    IPatientService patientService)
    : IRequestHandler<SendPatientSignInOtpCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SendPatientSignInOtpCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        return await patientService.SignIn(request.Email, cancellationToken);
    }
}