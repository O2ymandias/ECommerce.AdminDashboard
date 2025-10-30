using ECommerce.Core.Models.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Business;
internal class DeliveryMethodConfig : IEntityTypeConfiguration<DeliveryMethod>
{
	public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
	{
		builder.ToTable("DeliveryMethods");
		builder.HasKey(d => d.Id);

		builder
			.Property(d => d.ShortName)
			.HasMaxLength(50);

		builder
			.Property(d => d.DeliveryTime)
			.HasMaxLength(50);
		builder
			.Property(d => d.Description)
			.HasMaxLength(100);

		builder
			.Property(d => d.Cost)
			.HasColumnType("DECIMAL(18,2)");
	}
}
