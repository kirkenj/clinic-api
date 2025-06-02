using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public sealed class OtpRecord : BaseEntity, IAuditableEntity
{
    public CreatorType CreatorType { get; private set; }
    public OtpOperationPurpose OperationPurpose { get; private set; }
    public uint CreatorId { get; private set; }
    public string OtpHash { get; private set; } = default!;
    public DateTimeOffset? ExpiresOnUtc { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }
}