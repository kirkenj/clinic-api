using Clinic.Application.Responses.Offices;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Offices.Queries.GetAll;
public sealed record GetAllOfficesQuery : IRequest<Result<IEnumerable<OfficeResponse>>>
{ }