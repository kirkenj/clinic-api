namespace Clinic.Domain;

public interface IAuditableEntity
{
    DateTimeOffset CreatedOnUtc { get; set; }
    DateTimeOffset? UpdatedOnUtc { get; set; }
}