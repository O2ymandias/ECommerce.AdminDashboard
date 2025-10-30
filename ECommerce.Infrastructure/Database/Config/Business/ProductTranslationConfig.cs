using System;
using ECommerce.Core.Common.Enums;
using ECommerce.Core.Models.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Business;

public class ProductTranslationConfig : IEntityTypeConfiguration<ProductTranslation>
{
    public void Configure(EntityTypeBuilder<ProductTranslation> builder)
    {
        builder.ToTable("ProductTranslations");
        builder.HasKey(x => x.Id);
        builder
            .HasIndex(x => new { x.ProductId, x.LanguageCode })
            .IsUnique();
        builder
            .Property(x => x.Name)
            .HasMaxLength(100);
        builder
            .Property(x => x.LanguageCode)
            .HasConversion(
                to => to.ToString(),
                from => Enum.Parse<LanguageCode>(from)
            );
        builder
            .HasOne(x => x.Product)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
