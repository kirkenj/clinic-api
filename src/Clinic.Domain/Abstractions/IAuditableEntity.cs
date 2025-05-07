namespace Clinic.Domain.Abstractions;

public interface IAuditableEntity
{
    DateTimeOffset CreatedOnUtc { get; set; }
    DateTimeOffset? UpdatedOnUtc { get; set; }
}