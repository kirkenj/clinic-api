using Clinic.Application.Responses.Receptionists;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Queries.GetAll;

public sealed class GetAllReceptionistsQuery : IRequest<Result<IEnumerable<ReceptionistResponse>>>
{
}