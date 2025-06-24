using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Commands.SignIn;

public class SendReceptionistSignInOtpCommand : IRequest<Result<string>>
{
    public string Email { get; set; } = default!;

    public void TrimStringProperties()
    {
        Email = Email.Trim();
    }
}