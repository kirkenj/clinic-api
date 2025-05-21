using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Persistence.Configurations;

public sealed class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
{
    public void Configure(EntityTypeBuilder<ServiceCategory> builder)
    {
        builder.ToTable("ServiceCategories");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .UseIdentityColumn();

        builder.Property(m => m.IsActive)
            .IsRequired();

        builder.Property(m => m.CreatedOnUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedOnUtc)
            .IsRequired(false);
    }
}