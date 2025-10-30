using ECommerce.Core.Models.OrderModule;
using ECommerce.Core.Models.ProductModule;
using ECommerce.Core.Models.WishlistModule;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Core.Models.AuthModule;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; }
    public string? PictureUrl { get; set; }
    public Address? Address { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Rating> Ratings { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<WishlistItem> WishlistItems { get; set; } = [];
}