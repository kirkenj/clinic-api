using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Commands.SignIn;

public class SendReceptionistSignInOtpCommandHandler(
    SendReceptionistSignInOtpCommandValidator validator,
    IReceptionistService receptionistService)
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

        return await receptionistService.SendSignInOtpAsync(request.Email, cancellationToken);
    }
}