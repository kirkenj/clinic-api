using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.ServiceCategories.Commands.Add;

public sealed class AddServiceCategoryCommand : IRequest<Result>
{
    public string Name { get; set; } = default!;
    public TimeSpan Duration { get; set; } = default!;


    public void TrimStringProperties()
    {
        Name = Name.Trim();
    }
}