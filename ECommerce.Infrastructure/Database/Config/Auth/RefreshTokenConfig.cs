using ECommerce.Core.Models.AuthModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Auth;
internal class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(EntityTypeBuilder<RefreshToken> builder)
	{
		builder.ToTable("RefreshTokens", schema: "auth");
		builder.HasKey(r => r.Id);
		builder.HasIndex(r => r.Token);
	}
}
