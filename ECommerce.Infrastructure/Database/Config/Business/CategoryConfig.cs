using ECommerce.Core.Models.CategoryModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Business;
internal class CategoryConfig : IEntityTypeConfiguration<Category>
{
	public void Configure(EntityTypeBuilder<Category> builder)
	{
		builder.ToTable("Categories");

		builder.HasKey(c => c.Id);

		builder.Property(c => c.Name)
			.HasMaxLength(50);
	}
}
