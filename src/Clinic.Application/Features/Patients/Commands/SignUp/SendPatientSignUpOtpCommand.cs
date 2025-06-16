using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.SignUp;

public sealed class SendPatientSignUpOtpCommand : IRequest<Result<string>>
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string MiddleName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;


    public void TrimStringProperties()
    {
        FirstName = FirstName.Trim();
        LastName = LastName.Trim();
        MiddleName = MiddleName.Trim();
        Email = Email.Trim();
        PhoneNumber = PhoneNumber.Trim();
    }
}