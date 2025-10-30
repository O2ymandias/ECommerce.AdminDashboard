using ECommerce.Core.Dtos.DashboardDtos.RolesDtos;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.APIs.Controllers.Admin
{
    public class RolesController(IRoleService roleService) : AdminController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<RoleResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<RoleResult>>> GetRoles()
        {
            var roles = await roleService.GetRolesAsync();
            return Ok(roles);
        }
    }
}