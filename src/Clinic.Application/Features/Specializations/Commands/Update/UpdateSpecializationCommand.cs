using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specializations.Commands.Update;

public sealed class UpdateSpecializationCommand : IRequest<Result>
{
    public uint Id { get; set; }
    public string Name { get; set; } = default!;

    public void TrimStringProperties()
    {
        Name = Name.Trim();
    }
}