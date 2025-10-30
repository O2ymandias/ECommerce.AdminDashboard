using ECommerce.Core.Common.Enums;
using ECommerce.Core.Models.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Business;
internal class DeliveryMethodTranslationConfig : IEntityTypeConfiguration<DeliveryMethodTranslation>
{
	public void Configure(EntityTypeBuilder<DeliveryMethodTranslation> builder)
	{
		builder.HasKey(x => x.Id);

		builder.ToTable("DeliveryMethodTranslations");

		builder
			.HasIndex(x => new { x.DeliveryMethodId, x.Id })
			.IsUnique();

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
			.Property(x => x.LanguageCode)
			.HasConversion(
				to => to.ToString(),
				from => Enum.Parse<LanguageCode>(from)
			);

		builder
			.HasOne(x => x.DeliveryMethod)
			.WithMany(x => x.Translations)
			.HasForeignKey(x => x.DeliveryMethodId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);
	}
}
