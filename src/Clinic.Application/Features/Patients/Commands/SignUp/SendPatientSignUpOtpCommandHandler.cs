using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.SignUp;

public sealed class SendPatientSignUpOtpCommandHandler(
    IPatientService patientService,
    SendPatientSignUpOtpCommandValidator validator)
    : IRequestHandler<SendPatientSignUpOtpCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SendPatientSignUpOtpCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        return await patientService.SignUp(request.FirstName, request.MiddleName, request.LastName,
            request.Email, request.PhoneNumber, cancellationToken);
    }
}