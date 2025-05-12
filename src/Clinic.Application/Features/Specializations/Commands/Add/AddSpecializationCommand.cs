using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specializations.Commands.Add;

public sealed record AddSpecializationCommand : IRequest<Result>
{
    public string Name { get; set; } = default!;

    public void TrimStringProperties()
    {
        Name = Name.Trim();
    }
}