using Clinic.Domain.Abstractions;
using Clinic.Domain.Speciallization;
using MediatR;

namespace Clinic.Application.Features.Speciallization.Commands.Delete;
public sealed class DeleteSpeciallizationCommandHandler(ISpeciallizationRepository speciallizationRepository) : IRequestHandler<DeleteSpeciallizationCommand, Result>
{
    public async Task<Result> Handle(DeleteSpeciallizationCommand request, CancellationToken cancellationToken)
    {
        var speciallization = await speciallizationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (speciallization == null)
        {
            return SpeciallizationErrors.NotFound(request.Id);
        }

        await speciallizationRepository.DeleteAsync(speciallization, cancellationToken);

        return Result.Success();
    }
}