using Clinic.Application.Dtos.Office;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Office.Queries.GetOfficeQuery;

public sealed record class GetOfficeQuery(uint Id) : IRequest<Result<OfficeDto>>
{
}