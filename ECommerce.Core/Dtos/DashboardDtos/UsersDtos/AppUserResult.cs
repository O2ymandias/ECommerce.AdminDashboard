using ECommerce.Core.Dtos.OrderDtos;

namespace ECommerce.Core.Dtos.DashboardDtos.UsersDtos;

public class AppUserResult
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PictureUrl { get; set; }
    public string[] Roles { get; set; }
}