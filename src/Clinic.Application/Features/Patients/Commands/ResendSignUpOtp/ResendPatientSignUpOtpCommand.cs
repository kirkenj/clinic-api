using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.ResendSignUpOtp;

public class ResendPatientSignUpOtpCommand : IRequest<Result>
{
    public string Email { get; set; } = default!;

    public void TrimStringProperties()
    {
        Email = Email.Trim();
    }
}