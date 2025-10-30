using ECommerce.Core.Common.Enums;
using ECommerce.Core.Models.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Database.Config.Business;

internal class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);

        builder
            .Property(o => o.OrderStatus)
            .HasConversion(
                to => to.ToString(),
                from => Enum.Parse<OrderStatus>(from));

        builder
            .Property(o => o.PaymentMethod)
            .HasConversion(
                to => to.ToString(),
                from => Enum.Parse<PaymentMethod>(from));

        builder
            .Property(o => o.PaymentStatus)
            .HasConversion(
                to => to.ToString(),
                from => Enum.Parse<PaymentStatus>(from));

        builder
            .OwnsOne(o => o.ShippingAddress, owned =>
            {
                owned.WithOwner();

                owned
                    .Property(x => x.RecipientName)
                    .HasMaxLength(50);

                owned
                    .Property(x => x.PhoneNumber)
                    .HasMaxLength(50);

                owned
                    .Property(x => x.Street)
                    .HasMaxLength(50);

                owned
                    .Property(x => x.City)
                    .HasMaxLength(50);

                owned
                    .Property(x => x.Country)
                    .HasMaxLength(50);
            });

        builder
            .HasOne(o => o.DeliveryMethod)
            .WithMany()
            .IsRequired(false)
            .HasForeignKey(o => o.DeliveryMethodId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(o => o.User)
            .WithMany(o => o.Orders)
            .IsRequired()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(o => o.SubTotal)
            .HasColumnType("DECIMAL(18,2)");
    }
}