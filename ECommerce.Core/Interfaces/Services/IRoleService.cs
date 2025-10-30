using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.RolesDtos;

namespace ECommerce.Core.Interfaces.Services;

public interface IRoleService
{
    Task<IReadOnlyList<RoleResult>> GetRolesAsync();
}