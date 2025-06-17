using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Persistence.Configurations;

public sealed class AppointmentResultConfiguration : IEntityTypeConfiguration<AppointmentResult>
{
    public void Configure(EntityTypeBuilder<AppointmentResult> builder)
    {
        builder.ToTable("AppointmentResults");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .UseIdentityColumn();

        builder.HasOne(m => m.Appointment)
            .WithOne(s => s.AppointmentResult)
            .HasForeignKey<AppointmentResult>(m => m.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.Conclusion)
            .IsRequired();

        builder.Property(m => m.Complaints)
            .IsRequired();

        builder.Property(m => m.Recommendations)
            .IsRequired();

        builder.Property(m => m.CreatedOnUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedOnUtc)
            .IsRequired(false);
    }
}