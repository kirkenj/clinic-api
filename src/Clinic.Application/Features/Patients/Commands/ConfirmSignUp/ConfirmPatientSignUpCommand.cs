using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.ConfirmSignUp;

public sealed class ConfirmPatientSignUpCommand : IRequest<Result>
{
    public string Email { get; set; } = default!;
    public string Otp { get; set; } = default!;

    public void TrimStringProperties()
    {
        Email = Email.Trim();
        Otp = Otp.Trim();
    }
}