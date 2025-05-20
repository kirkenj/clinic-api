using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.ServiceCategories.Commands.Delete;

public sealed class DeleteServiceCategoryCommand : IRequest<Result>
{
    public uint Id { get; set; }
}