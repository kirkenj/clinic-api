using Clinic.Application.Responses.Offices;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Offices.Queries.GetAll;
public sealed class GetAllOfficesQuery : IRequest<Result<IEnumerable<OfficeResponse>>>
{ }