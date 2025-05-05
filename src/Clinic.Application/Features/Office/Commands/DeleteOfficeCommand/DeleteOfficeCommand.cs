using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Office.Commands.DeleteOfficeCommand;

public sealed record class DeleteOfficeCommand(
    uint Id
    ) : IRequest<Result>
{
}