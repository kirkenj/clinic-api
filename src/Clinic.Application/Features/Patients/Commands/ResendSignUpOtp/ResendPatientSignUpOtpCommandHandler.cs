using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.ResendSignUpOtp;

public sealed class ResendPatientSignUpOtpCommandHandler(
    ResendPatientSignUpOtpCommandValidator validator,
    IPatientService patientService)
    : IRequestHandler<ResendPatientSignUpOtpCommand, Result>
{
    public async Task<Result> Handle(ResendPatientSignUpOtpCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors
                .Select(e => e.ErrorMessage));
        }

        return await patientService.ResendSignUpOtp(request.Email, cancellationToken);
    }
}