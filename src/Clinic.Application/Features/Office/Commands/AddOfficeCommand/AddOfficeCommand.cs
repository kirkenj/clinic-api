using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Office.Commands.AddOfficeCommand;

public sealed record class AddOfficeCommand(
    string Address, 
    string RegistryPhoneNumber, 
    string Photo, 
    string PhotoId) : IRequest<Result>
{
}