using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.SignIn;

public class PatientSignInRequest : IRequest<Result<string>>
{
    public string Email { get; private set; } = default!;

    public void TrimStringProperties()
    {
        Email = Email.Trim();
    }
}