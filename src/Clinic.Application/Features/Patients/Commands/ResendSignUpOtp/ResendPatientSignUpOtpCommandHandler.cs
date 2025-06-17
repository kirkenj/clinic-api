using Clinic.Application.Abstractions;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.ResendSignUpOtp;

public sealed class ResendPatientSignUpOtpCommandHandler(
    ResendPatientSignUpOtpCommandValidator validator,
    IOtpService otpService,
    IPatientRepository patientRepository,
    IEmailService emailService) 
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

        var patient = await patientRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(request.Email);
        }

        if (patient.IsActive)
        {
            return PatientErrors.Active(patient.Id);
        }

        var otpKey = $"AddPatientCommand:{request.Email}";
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return otpGenerationResult.Error;
        }

        await emailService.SendAsync(patient.Email, Constants.SubjectSignUp,
            EmailBodyBuilder.BuildSignUpContent(otpGenerationResult.Value), cancellationToken);
        return Result.Success();
    }
}