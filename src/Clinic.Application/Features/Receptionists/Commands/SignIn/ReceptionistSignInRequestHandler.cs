using Clinic.Application.Abstractions;
using Clinic.Application.Email;
using Clinic.Application.Otp;
using Clinic.Application.Responses;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Commands.SignIn;

public class ReceptionistSignInRequestHandler(
    ReceptionistSignInRequestValidator validator,
    IOtpService otpService,
    IReceptionistRepository receptionistRepository,
    IEmailService emailService) : IRequestHandler<ReceptionistSignInRequest, Result<string>>
{
    public async Task<Result<string>> Handle(ReceptionistSignInRequest request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var receptionist = await receptionistRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (receptionist is null)
        {
            return Result.Failure<string>(ReceptionistErrors.NotFound(request.Email));
        }

        if (!receptionist.IsActive)
        {
            return Result.Failure<string>(ReceptionistErrors.NotActive(receptionist.Id));
        }

        var otpKey = OtpKeys.ReceptionistSignInRequest(receptionist.Email);
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        await emailService.SendAsync(receptionist.Email, EmailSubjects.SignIn, 
            EmailBodies.SignIn(otpGenerationResult.Value), cancellationToken);

        return Result.Success(StringResponses.CheckEmailForFurtherDetails);
    }
}