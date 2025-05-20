using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.ServiceCategories.Commands.Update;

public sealed class UpdateServiceCategoryCommand : IRequest<Result>
{
    public uint Id { get; set; }
    public string Name { get; set; } = default!;
    public TimeSpan Duration { get; set; } = default!;

    public void TrimStringProperties()
    {
        Name = Name.Trim();
    }
}