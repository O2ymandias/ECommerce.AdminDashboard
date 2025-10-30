using ECommerce.Core.Models.WishlistModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Business;
internal class WishlistItemConfig : IEntityTypeConfiguration<WishlistItem>
{
	public void Configure(EntityTypeBuilder<WishlistItem> builder)
	{
		builder.ToTable("WishlistItems");
		builder.HasKey(x => x.Id);
		builder
			.HasOne(x => x.User)
			.WithMany(x => x.WishlistItems)
			.HasForeignKey(x => x.UserId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(x => x.Product)
			.WithMany(x => x.WishlistItems)
			.HasForeignKey(x => x.ProductId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);
	}
}
