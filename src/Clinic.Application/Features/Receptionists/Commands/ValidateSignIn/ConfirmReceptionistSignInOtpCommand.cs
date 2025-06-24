using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Commands.ValidateSignIn;

public sealed class ConfirmReceptionistSignInOtpCommand : IRequest<Result<string>>
{
    public string Email { get; set; } = default!;
    public string Otp { get; set; } = default!;

    public void TrimStringProperties()
    {
        Email = Email.Trim();
        Otp = Otp.Trim();
    }
}