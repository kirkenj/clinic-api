using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.SignIn;

public class DoctorSignInRequest : IRequest<Result<string>>
{
    public string Email { get; set; } = default!;

    public void TrimStringProperties()
    {
        Email = Email.Trim();
    }
}