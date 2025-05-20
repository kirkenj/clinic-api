using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries.GetPaginated;

public class GetPaginatedDoctorsRequest : IRequest<Result<PaginatedCollection<DoctorResponse>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}