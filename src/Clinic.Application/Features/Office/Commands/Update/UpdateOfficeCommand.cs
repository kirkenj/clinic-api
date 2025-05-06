using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Office.Commands.Update;

public sealed record class UpdateOfficeCommand(
    uint Id,
    string Address,
    string RegistryPhoneNumber,
    string Photo,
    string PhotoId
    ) : IRequest<Result>
{ }