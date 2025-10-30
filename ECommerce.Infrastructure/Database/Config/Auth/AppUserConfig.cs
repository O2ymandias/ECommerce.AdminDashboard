using ECommerce.Core.Models.AuthModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Auth;
internal class AppUserConfig : IEntityTypeConfiguration<AppUser>
{
	public void Configure(EntityTypeBuilder<AppUser> builder)
	{

		builder
			.Property(u => u.DisplayName)
			.HasMaxLength(50);

		builder
			.Property(u => u.UserName)
			.HasMaxLength(50);

		builder
			.HasOne(u => u.Address)
			.WithOne()
			.HasForeignKey<Address>(a => a.AppUserId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasMany(u => u.RefreshTokens)
			.WithOne()
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);
	}
}
