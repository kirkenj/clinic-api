using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.ValidateSignIn;

public sealed class ConfirmDoctorSignInOtpCommand : IRequest<Result<string>>
{
    public string Email { get; private set; } = default!;
    public string Otp { get; private set; } = default!;

    public void TrimStringProperties()
    {
        Email = Email.Trim();
        Otp = Otp.Trim();
    }
}