using Clinic.Domain.Speciallization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Persistence.Configurations;
public sealed class SpeciallizationConfiguration : IEntityTypeConfiguration<Speciallization>
{
    public void Configure(EntityTypeBuilder<Speciallization> builder)
    {
        builder.ToTable("Speciallizations");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .UseIdentityColumn();

        builder.Property(m => m.Name)
            .IsRequired();

        builder.HasIndex(m => m.Name)
            .IsUnique();

        builder.Property(m => m.IsActive)
            .IsRequired();

        builder.Property(m => m.CreatedOnUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedOnUtc)
            .IsRequired(false);
    }
}