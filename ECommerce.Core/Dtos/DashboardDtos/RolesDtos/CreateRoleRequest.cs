using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.DashboardDtos.RolesDtos;

public class CreateRoleRequest
{
    private string _roleName;

    [Required]
    [MaxLength(50)]
    public string RoleName
    {
        get => _roleName;
        set => _roleName = value.Trim();
    }
}