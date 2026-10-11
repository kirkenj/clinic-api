using Clinic.Application.Responses.Receptionists;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Queries.GetCurrent;

public sealed class GetCurrentReceptionistQuery : IRequest<Result<ReceptionistResponse>>{ }