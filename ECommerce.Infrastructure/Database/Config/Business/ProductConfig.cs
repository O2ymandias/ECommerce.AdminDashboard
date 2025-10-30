using ECommerce.Core.Models.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Business;

internal class ProductConfig : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.ToTable("Products");

		builder.HasKey(p => p.Id);

		builder.HasIndex(p => p.Name);

		builder
			.Property(p => p.Name)
			.HasMaxLength(100);

		builder
			.Property(p => p.Price)
			.HasColumnType("DECIMAL(18,2)");

		builder
			.HasOne(p => p.Brand)
			.WithMany()
			.HasForeignKey(p => p.BrandId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(p => p.Category)
			.WithMany()
			.HasForeignKey(p => p.CategoryId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);
	}
}
