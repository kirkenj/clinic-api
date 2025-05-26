using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.DapperPersistence.Configurations;

public sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .UseIdentityColumn();

        builder.Property(m => m.IsActive)
            .IsRequired();
        
        builder.Property(m => m.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(m => m.Name)
            .IsUnique();
        
        builder.Property(m => m.Price)
            .IsRequired();

        builder.Property(m => m.SpecializationId)
            .IsRequired();

        builder.HasOne(m => m.Specialization)
            .WithMany(s => s.Services)
            .HasForeignKey(m => m.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.ServiceCategoryId)
            .IsRequired();

        builder.HasOne(m => m.ServiceCategory)
            .WithMany(s => s.Services)
            .HasForeignKey(m => m.ServiceCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.CreatedOnUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedOnUtc)
            .IsRequired(false);
    }
}