using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Persistence.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .UseIdentityColumn();

        builder.Property(m => m.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.MiddleName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.IsActive)
            .IsRequired();

        builder.Property(m => m.PhoneNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(m => m.PhoneNumber)
            .IsUnique();

        builder.Property(m => m.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(m => m.Email)
            .IsUnique();

        builder.Property(m => m.CreatedOnUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedOnUtc)
            .IsRequired(false);
    }
}