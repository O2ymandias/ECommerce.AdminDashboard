using ECommerce.Core.Common.Enums;
using ECommerce.Core.Models.CategoryModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Business;
internal class CategoryTranslationConfig : IEntityTypeConfiguration<CategoryTranslation>
{
	public void Configure(EntityTypeBuilder<CategoryTranslation> builder)
	{
		builder.HasKey(x => x.Id);

		builder.ToTable("CategoryTranslations");

		builder
			.Property(x => x.Name)
			.HasMaxLength(50);

		builder
			.HasIndex(x => new { x.CategoryId, x.Id })
			.IsUnique();

		builder
			.Property(x => x.LanguageCode)
			.HasConversion(
				to => to.ToString(),
				from => Enum.Parse<LanguageCode>(from)
			);

		builder
			.HasOne(x => x.Category)
			.WithMany(x => x.Translations)
			.HasForeignKey(x => x.CategoryId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);
	}
}
