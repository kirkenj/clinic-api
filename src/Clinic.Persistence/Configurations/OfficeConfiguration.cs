using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Persistence.Configurations;

public sealed class OfficeConfiguration : IEntityTypeConfiguration<Office>
{
    public void Configure(EntityTypeBuilder<Office> builder)
    {
        builder.ToTable("Offices");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .UseIdentityColumn();

        builder.Property(m => m.Address)
            .IsRequired();

        builder.Property(m => m.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(m => m.PhoneNumber)
            .IsUnique();

        builder.Property(m => m.IsActive)
            .IsRequired();

        builder.Property(m => m.CreatedOnUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedOnUtc)
            .IsRequired(false);
    }
}