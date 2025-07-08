using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.SignIn;

public class SendDoctorSignInOtpCommandHandler(
    SendDoctorSignInOtpCommandValidator validator,
    IDoctorService doctorService)
    : IRequestHandler<SendDoctorSignInOtpCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SendDoctorSignInOtpCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        return await doctorService.SendSignInOtpAsync(request.Email, cancellationToken);
    }
}