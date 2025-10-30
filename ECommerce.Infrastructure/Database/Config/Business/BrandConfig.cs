using ECommerce.Core.Models.BrandModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Business;

internal class BrandConfig : IEntityTypeConfiguration<Brand>
{
	public void Configure(EntityTypeBuilder<Brand> builder)
	{
		builder.ToTable("Brands");

		builder.HasKey(b => b.Id);

		builder.Property(b => b.Name)
			.HasMaxLength(50);
	}
}
