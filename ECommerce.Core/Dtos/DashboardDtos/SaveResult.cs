using Newtonsoft.Json;

namespace ECommerce.Core.Dtos.DashboardDtos;

public class SaveResult
{
    public bool Success { get; set; }

    [JsonIgnore]
    public int StatusCode { get; set; }
    public string Message { get; set; }
}