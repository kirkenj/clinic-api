using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.ConfirmSignIn;

public sealed class ConfirmPatientSignInOtpCommand : IRequest<Result<string>>
{
    public string Email { get; private set; } = default!;
    public string Otp { get; private set; } = default!;

    public void TrimStringProperties()
    {
        Email = Email.Trim();
        Otp = Otp.Trim();
    }
}