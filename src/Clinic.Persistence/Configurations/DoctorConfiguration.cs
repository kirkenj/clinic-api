using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.DapperPersistence.Configurations;

public sealed class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

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

        builder.Property(m => m.SpecializationId)
            .IsRequired();

        builder.HasOne(m => m.Specialization)
            .WithMany(m => m.Doctors)
            .HasForeignKey(m => m.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.OfficeId)
            .IsRequired();

        builder.HasOne(m => m.Office)
            .WithMany(m => m.Doctors)
            .HasForeignKey(m => m.OfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.CreatedOnUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedOnUtc)
            .IsRequired(false);
    }
}