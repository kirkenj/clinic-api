using Clinic.Application.Responses.Offices;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Offices.Queries.Get;

public sealed record GetOfficeQuery(uint Id) : IRequest<Result<OfficeResponse>>
{ }