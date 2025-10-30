using ECommerce.Core.Models.AuthModule;
using ECommerce.Core.Models.BrandModule;
using ECommerce.Core.Models.CategoryModule;
using ECommerce.Core.Models.ProductModule;
using ECommerce.Infrastructure.Database.Config.Auth;
using ECommerce.Infrastructure.Database.Config.Business;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Database;

public class AppDbContext : IdentityDbContext<AppUser>
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	protected AppDbContext()
	{
	}

	public DbSet<Product> Products { get; set; }
	public DbSet<Brand> Brands { get; set; }
	public DbSet<Category> Categories { get; set; }
	public DbSet<ProductGallery> ProductGalleries { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		// Business
		builder.ApplyConfiguration(new ProductConfig());
		builder.ApplyConfiguration(new BrandConfig());
		builder.ApplyConfiguration(new CategoryConfig());
		builder.ApplyConfiguration(new ProductGalleryConfig());
		builder.ApplyConfiguration(new RatingConfig());
		builder.ApplyConfiguration(new OrderConfig());
		builder.ApplyConfiguration(new OrderItemConfig());
		builder.ApplyConfiguration(new DeliveryMethodConfig());
		builder.ApplyConfiguration(new ProductTranslationConfig());
		builder.ApplyConfiguration(new WishlistItemConfig());
		builder.ApplyConfiguration(new BrandTranslationConfig());
		builder.ApplyConfiguration(new CategoryTranslationConfig());
		builder.ApplyConfiguration(new DeliveryMethodTranslationConfig());

		// Identity
		base.OnModelCreating(builder);
		builder.Entity<AppUser>().ToTable("Users", schema: "auth");
		builder.Entity<IdentityRole>().ToTable("Roles", schema: "auth");
		builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", schema: "auth");
		builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", schema: "auth");
		builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", schema: "auth");
		builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", schema: "auth");
		builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", schema: "auth");
		builder.ApplyConfiguration(new AppUserConfig());
		builder.ApplyConfiguration(new AddressConfig());
		builder.ApplyConfiguration(new RefreshTokenConfig());
	}
}
