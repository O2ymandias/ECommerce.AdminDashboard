using ECommerce.Core.Models.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace ECommerce.Infrastructure.Database.Config.Business;

internal class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(x => x.Id);


        builder
            .OwnsOne(x => x.Product, owned =>
            {
                owned.WithOwner();
                owned
                    .Property(x => x.Name)
                    .HasMaxLength(100);

                owned
                    .Property(x => x.NameTranslations)
                    .HasConversion(
                        to => JsonConvert.SerializeObject(to),
                        from => JsonConvert.DeserializeObject<Dictionary<string, string>>(from) ??
                                new Dictionary<string, string>()
                    );
            });

        builder
            .Property(x => x.Price)
            .HasColumnType("DECIMAL(18,2)");
    }
}