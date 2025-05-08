using Clinic.Domain.Abstractions;
using Clinic.Domain.Specialization;
using MediatR;

namespace Clinic.Application.Features.Specialization.Commands.Delete;
public sealed class DeleteSpecializationCommandHandler(ISpecializationRepository specializationRepository) 
    : IRequestHandler<DeleteSpecializationCommand, Result>
{
    public async Task<Result> Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await specializationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (specialization == null)
        {
            return SpecializationErrors.NotFound(request.Id);
        }

        await specializationRepository.DeleteAsync(specialization, cancellationToken);

        return Result.Success();
    }
}