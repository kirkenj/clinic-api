using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Office.Commands.Delete;

public sealed record class DeleteOfficeCommand(
    uint Id
    ) : IRequest<Result>
{ }