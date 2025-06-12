using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.ValidateSignIn;

public sealed class DoctorValidateSignInRequest : IRequest<Result<string>>
{
    public string Email { get; set; } = default!;
    public string Otp { get; set; } = default!;

    public void TrimStringProperties()
    {
        Email = Email.Trim();
        Otp = Otp.Trim();
    }
}