using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Commands.Update;

public sealed class UpdateReceptionistCommand : IRequest<Result>
{
    public uint Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;

    public void TrimStringProperties()
    {
        FirstName = FirstName.Trim();
        LastName = LastName.Trim();
        MiddleName = MiddleName.Trim();
        Email = Email.Trim();
        PhoneNumber = PhoneNumber.Trim();
    }
}