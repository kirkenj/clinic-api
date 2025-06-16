using Clinic.Application.Abstractions;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Commands.SignIn;

public class SendReceptionistSignInOtpCommandHandler(
    SendReceptionistSignInOtpCommandValidator validator,
    IOtpService otpService,
    IReceptionistRepository receptionistRepository,
    IEmailService emailService) 
    : IRequestHandler<SendReceptionistSignInOtpCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SendReceptionistSignInOtpCommand request,
        CancellationToken cancellationToken)
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

        var otpKey = $"ReceptionistSignInRequest:{receptionist.Email}";
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        await emailService.SendAsync(receptionist.Email, Constants.SubjectSignIn,
            EmailBodyBuilder.BuildSignInContent(otpGenerationResult.Value), cancellationToken);

        return Result.Success(Constants.ResponseCheckEmailForFurtherDetails);
    }
}