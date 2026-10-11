using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Commands.ValidateSignIn;

public sealed class ConfirmReceptionistSignInOtpCommandHandler
    (ConfirmReceptionistSignInOtpCommandValidator validator,
    IReceptionistService receptionistService)
    : IRequestHandler<ConfirmReceptionistSignInOtpCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConfirmReceptionistSignInOtpCommand request,
        CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        return await receptionistService.ValidateSignInAsync(request.Email, request.Otp, cancellationToken);
    }
}