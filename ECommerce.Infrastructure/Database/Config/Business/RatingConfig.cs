using ECommerce.Core.Models.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Business;
public class RatingConfig : IEntityTypeConfiguration<Rating>
{
	public void Configure(EntityTypeBuilder<Rating> builder)
	{
		builder.ToTable("Ratings");
		builder.HasKey(r => r.Id);
		builder.HasIndex(r => r.UserId);
		builder.HasIndex(r => r.ProductId);

		builder
			.Property(r => r.Comment)
			.HasMaxLength(500);

		builder
			.Property(r => r.Title)
			.HasMaxLength(50);

		// Product (1) -> Rating (M) 
		builder
			.HasOne(r => r.Product)
			.WithMany(p => p.Ratings)
			.HasForeignKey(r => r.ProductId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);

		// User (1) -> Rating (M) 
		builder
			.HasOne(r => r.User)
			.WithMany(u => u.Ratings)
			.HasForeignKey(r => r.UserId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);
	}
}
