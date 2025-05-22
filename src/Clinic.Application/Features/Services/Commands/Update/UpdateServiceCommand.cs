using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Services.Commands.Update;

public sealed class UpdateServiceCommand : IRequest<Result>
{
    public uint Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public uint SpecializationId { get; set; }
    public uint ServiceCategoryId { get; set; }

    public void TrimStringProperties()
    {
        Name = Name.Trim();
    }
}