using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Offices.Commands.Add;

public sealed class AddOfficeCommand : IRequest<Result>
{
    public string Address { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;


    public void TrimStringProperties()
    {
        Address = Address.Trim();
        PhoneNumber = PhoneNumber.Trim();
    }
}