using Clinic.Application.Responses.Receptionists;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Queries.Get;

public sealed record GetReceptionistQuery(uint Id) : IRequest<Result<ReceptionistResponse>>
{ }