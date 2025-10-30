using System;
using ECommerce.Core.Common.Enums;
using ECommerce.Core.Models.BrandModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Business;

public class BrandTranslationConfig : IEntityTypeConfiguration<BrandTranslation>
{
    public void Configure(EntityTypeBuilder<BrandTranslation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.ToTable("BrandTranslations");

        builder
            .Property(x => x.Name)
            .HasMaxLength(50);

        builder
            .HasIndex(x => new { x.BrandId, x.Id })
            .IsUnique();

        builder
            .Property(x => x.LanguageCode)
            .HasConversion(
                to => to.ToString(),
                from => Enum.Parse<LanguageCode>(from)
            );

        builder
            .HasOne(x => x.Brand)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.BrandId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
