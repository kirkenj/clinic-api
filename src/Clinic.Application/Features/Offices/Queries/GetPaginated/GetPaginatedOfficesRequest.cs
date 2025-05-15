using Clinic.Application.Responses.Offices;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Offices.Queries.GetPaginated;
public class GetPaginatedOfficesRequest : IRequest<Result<IEnumerable<OfficeResponse>>>
{ 
    public uint Page { get; set; }    
    public uint PageSize { get; set; }    
}