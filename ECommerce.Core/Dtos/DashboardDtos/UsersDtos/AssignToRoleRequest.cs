using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.DashboardDtos.UsersDtos;

public class AssignToRoleRequest
{
    [Required] public string UserId { get; set; }
    [Required] public string[] Roles { get; set; }
}