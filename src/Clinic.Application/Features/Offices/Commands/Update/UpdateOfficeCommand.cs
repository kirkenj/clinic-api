using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Offices.Commands.Update;

public sealed class UpdateOfficeCommand : IRequest<Result>
{
    public uint Id { get; set; }
    public string Address { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;

    public void TrimStringProperties()
    {
        Address = Address.Trim();
        PhoneNumber = PhoneNumber.Trim();
    }
}