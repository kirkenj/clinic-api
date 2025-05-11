using Clinic.Application.Responses.Office;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Office.Queries.GetAll;
public sealed record GetAllOfficesQuery : IRequest<Result<IEnumerable<OfficeResponse>>>
{ }