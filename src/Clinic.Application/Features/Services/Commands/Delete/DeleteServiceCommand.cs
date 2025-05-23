using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Services.Commands.Delete;

public sealed class DeleteServiceCommand : IRequest<Result>
{
    public uint Id { get; set; }
}