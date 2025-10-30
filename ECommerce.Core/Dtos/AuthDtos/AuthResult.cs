//using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace ECommerce.Core.Dtos.AuthDtos;

public class AuthResult
{
    public string Message { get; set; }
    public string Token { get; set; }
    public DateTime RefreshTokenExpiresOn { get; set; }
    public List<string> Roles { get; set; } = [];

    [JsonIgnore] public string RefreshToken { get; set; }

    [JsonIgnore] public int Status { get; set; }

    [JsonIgnore] public List<string> Errors { get; set; }
}