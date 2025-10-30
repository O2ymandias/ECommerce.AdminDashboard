using ECommerce.Core.Models.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Business;

internal class ProductGalleryConfig : IEntityTypeConfiguration<ProductGallery>
{
	public void Configure(EntityTypeBuilder<ProductGallery> builder)
	{
		builder.ToTable("ProductGalleries");

		builder.HasKey(g => g.Id);

		builder
			.Property(g => g.AltText)
			.IsRequired(false)
			.HasMaxLength(50);

		builder
			.HasOne(g => g.Product)
			.WithMany(p => p.Galleries)
			.HasForeignKey(g => g.ProductId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
