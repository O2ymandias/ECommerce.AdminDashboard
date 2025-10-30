using ECommerce.Core.Dtos.AuthDtos;

namespace ECommerce.Core.Dtos.ProfileDtos;

public class AccountInfoResult
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PictureUrl { get; set; }
    public AddressResult? Address { get; set; }
}