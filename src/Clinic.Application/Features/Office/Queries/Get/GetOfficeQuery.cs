using Clinic.Application.Dtos.Office;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Office.Queries.Get;

public sealed record GetOfficeQuery(uint Id) : IRequest<Result<OfficeResponse>>
{ }