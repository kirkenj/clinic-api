using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.ValidateSignUp;

public sealed class PatientValidateSignUpRequest : IRequest<Result>
{
    public string Email { get; private set; } = default!;
    public string Otp { get; private set; } = default!;

    public void TrimStringProperties()
    {
        Email = Email.Trim();
        Otp = Otp.Trim();
    }
}