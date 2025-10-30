using ECommerce.Core.Models.AuthModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Auth;
internal class AddressConfig : IEntityTypeConfiguration<Address>
{
	public void Configure(EntityTypeBuilder<Address> builder)
	{
		builder.ToTable("Addresses", schema: "auth");
		builder.HasKey(a => a.Id);

		builder.Property(a => a.Street)
			.IsRequired()
			.HasMaxLength(50);

		builder.Property(a => a.City)
			.IsRequired()
			.HasMaxLength(50);

		builder.Property(a => a.Country)
			.IsRequired()
			.HasMaxLength(50);
	}
}
