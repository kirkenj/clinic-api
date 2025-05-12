using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Specializations.Commands.Delete;
public sealed class DeleteSpecializationCommandHandler(ISpecializationRepository specializationRepository)
    : IRequestHandler<DeleteSpecializationCommand, Result>
{
    public async Task<Result> Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await specializationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (specialization is null)
        {
            return SpecializationErrors.NotFound(request.Id);
        }

        await specializationRepository.DeleteAsync(specialization, cancellationToken);

        return Result.Success();
    }
}