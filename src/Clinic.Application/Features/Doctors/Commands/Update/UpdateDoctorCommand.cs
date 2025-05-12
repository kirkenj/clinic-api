using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.Update;

public sealed class UpdateDoctorCommand : IRequest<Result>
{
    public uint Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public uint SpecializationId { get; set; }
    public uint OfficeId { get; set; }


    public void TrimStringProperties()
    {
        FirstName = FirstName.Trim();
        LastName = LastName.Trim();
        MiddleName = MiddleName.Trim();
        Email = Email.Trim();
        PhoneNumber = PhoneNumber.Trim();
    }
}