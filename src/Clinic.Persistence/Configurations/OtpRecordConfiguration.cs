using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Persistence.Configurations;

public sealed class OtpRecordConfiguration : IEntityTypeConfiguration<OtpRecord>
{
    public void Configure(EntityTypeBuilder<OtpRecord> builder)
    {
        builder.ToTable("OtpRecords");

        builder.HasKey(m => new { m.Key, m.CreatedOnUtc });

        builder.Property(m => m.Key)
            .IsRequired();

        builder.Property(m => m.Otp)
            .IsRequired();
        
        builder.Property(m => m.TriesCount)
            .IsRequired();
        
        builder.Property(m => m.MaxTriesCount)
            .IsRequired();

        builder.Property(m => m.IsActive)
            .IsRequired();

        builder.Property(m => m.ExpiresOnUtc)
            .IsRequired();
        
        builder.Property(m => m.CreatedOnUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedOnUtc)
            .IsRequired(false);
    }
}