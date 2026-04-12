using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.Status).IsRequired();
        builder.Property(p => p.Price)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(p => p.Stock).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
    }
}
