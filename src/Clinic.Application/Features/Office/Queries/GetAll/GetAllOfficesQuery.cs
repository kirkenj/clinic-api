using Clinic.Application.Dtos.Office;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Office.Queries.GetAll;
public sealed class GetAllOfficesQuery : IRequest<Result<IEnumerable<OfficeResponse>>>
{ }