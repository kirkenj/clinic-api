using Clinic.Application.Responses.Patients;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Filters;
using MediatR;

namespace Clinic.Application.Features.Patients.Queries.GetPaginated;

public class GetPaginatedPatientsRequest : IRequest<Result<PaginatedCollection<PatientResponse>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public PatientFilter? PatientFilter { get; set; }
}