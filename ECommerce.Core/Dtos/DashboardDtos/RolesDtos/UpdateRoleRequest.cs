using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.DashboardDtos.RolesDtos;

public class UpdateRoleRequest
{
    private string _newRoleName;

    [Required] public string RoleId { get; set; }

    [Required]
    [MaxLength(50)]
    public string NewRoleName
    {
        get => _newRoleName;
        set => _newRoleName = value.Trim();
    }
}