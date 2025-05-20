using FluentValidation;

namespace Clinic.Application.Features.Receptionists.Queries.GetPaginated;

public class GetPaginatedReceptionistsRequestValidator : AbstractValidator<GetPaginatedReceptionistsRequest>
{
    public GetPaginatedReceptionistsRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}