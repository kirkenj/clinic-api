using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .UseIdentityColumn();

        builder.Property(m => m.DoctorId)
            .IsRequired();

        builder.HasOne(m => m.Doctor)
            .WithMany(s => s.Appointments)
            .HasForeignKey(m => m.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.PatientId)
            .IsRequired();

        builder.HasOne(m => m.Patient)
            .WithMany(s => s.Appointments)
            .HasForeignKey(m => m.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.DateTimeStart)
            .IsRequired();

        builder.Property(m => m.DateTimeEnd)
            .IsRequired();

        builder.Property(m => m.IsApprovedByUser)
            .IsRequired();

        builder.Property(m => m.IsApprovedByClinic)
            .IsRequired();

        builder.Property(m => m.Status)
            .IsRequired();

        builder.Property(m => m.CreatedOnUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedOnUtc)
            .IsRequired(false);
    }
}