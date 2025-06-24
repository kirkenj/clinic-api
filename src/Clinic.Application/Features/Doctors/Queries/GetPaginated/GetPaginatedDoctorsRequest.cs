using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Filters;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries.GetPaginated;

public class GetPaginatedDoctorsRequest : IRequest<Result<PaginatedCollection<DoctorResponse>>>
{
    public DoctorFilter? DoctorFilter { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}