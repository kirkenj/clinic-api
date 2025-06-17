using Clinic.Application.Abstractions;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.SignIn;

public class SendDoctorSignInOtpCommandHandler(
    SendDoctorSignInOtpCommandValidator validator,
    IOtpService otpService,
    IDoctorRepository doctorRepository,
    IEmailService emailService) : IRequestHandler<SendDoctorSignInOtpCommand, Result<string>>
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

        var doctor = await doctorRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure<string>(DoctorErrors.NotFound(request.Email));
        }

        if (!doctor.IsActive)
        {
            return Result.Failure<string>(DoctorErrors.NotActive(doctor.Id));
        }

        var otpKey = $"DoctorSignInRequest:{doctor.Email}";
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        await emailService.SendAsync(doctor.Email, Constants.SubjectSignIn,
            EmailBodyBuilder.BuildSignInContent(otpGenerationResult.Value), cancellationToken);

        return Result.Success(Constants.ResponseCheckEmailForFurtherDetails);
    }
}