using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Offices.Commands.Delete;

public sealed class DeleteOfficeCommand : IRequest<Result>
{
    public uint Id { get; set; }
}